using EducProject.API.Data;
using EducProject.API.Models;
using EducProject.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EducProject.API.Services
{
    public class ProgressService : IProgressService
    {
        private readonly EducProjectContext _context;

        public ProgressService(EducProjectContext context)
        {
            _context = context;
        }

        public async Task<ProgressDto?> GetProgressByIdAsync(int id)
        {
            var progress = await _context.Progresses
                .Include(p => p.User)
                .Include(p => p.Module)
                .FirstOrDefaultAsync(p => p.Id == id);

            return progress != null ? MapToProgressDto(progress) : null;
        }

        public async Task<List<ProgressDto>> GetProgressByUserIdAsync(int userId)
        {
            var progressList = await _context.Progresses
                .Include(p => p.User)
                .Include(p => p.Module)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();

            return progressList.Select(MapToProgressDto).ToList();
        }

        public async Task<List<ProgressDto>> GetProgressByParentIdAsync(int parentId)
        {
            var progressList = await _context.Progresses
                .Include(p => p.User)
                .Include(p => p.Module)
                .Where(p => p.User.ParentId == parentId)
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();

            return progressList.Select(MapToProgressDto).ToList();
        }

        public async Task<ProgressDto> CreateProgressAsync(CreateProgressDto createDto)
        {
            var module = await _context.Modules.FindAsync(createDto.ModuleId);
            if (module == null)
                throw new InvalidOperationException("Module not found");

            var progress = new Progress
            {
                UserId = createDto.UserId,
                ModuleId = createDto.ModuleId,
                Status = createDto.Status,
                Score = createDto.Score,
                MaxScore = module.MaxScore,
                TimeSpentMinutes = createDto.TimeSpentMinutes,
                StartedAt = createDto.Status == ProgressStatus.InProgress ? DateTime.UtcNow : null,
                Attempts = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Progresses.Add(progress);
            await _context.SaveChangesAsync();

            // Check for badges
            await CheckAndAwardBadgesAsync(createDto.UserId, progress);

            return await GetProgressByIdAsync(progress.Id) ?? throw new InvalidOperationException("Failed to create progress");
        }

        public async Task<ProgressDto> UpdateProgressAsync(int id, UpdateProgressDto updateDto)
        {
            var progress = await _context.Progresses.FindAsync(id);
            if (progress == null)
                throw new InvalidOperationException("Progress not found");

            progress.Status = updateDto.Status;
            progress.Score = updateDto.Score;
            progress.TimeSpentMinutes = updateDto.TimeSpentMinutes;
            progress.Attempts = updateDto.Attempts;
            progress.UpdatedAt = DateTime.UtcNow;

            if (updateDto.Status == ProgressStatus.InProgress && progress.StartedAt == null)
            {
                progress.StartedAt = DateTime.UtcNow;
            }

            if (updateDto.Status == ProgressStatus.Completed && progress.CompletedAt == null)
            {
                progress.CompletedAt = DateTime.UtcNow;
                await CreateNotificationAsync(progress.UserId, "Module Complété!", 
                    $"Félicitations ! Vous avez complété le module avec un score de {updateDto.Score}%", 
                    NotificationType.ModuleCompleted, NotificationPriority.High, progress.Id);
            }

            await _context.SaveChangesAsync();

            // Check for badges
            await CheckAndAwardBadgesAsync(progress.UserId, progress);

            return await GetProgressByIdAsync(progress.Id) ?? throw new InvalidOperationException("Failed to update progress");
        }

        public async Task<bool> DeleteProgressAsync(int id)
        {
            var progress = await _context.Progresses.FindAsync(id);
            if (progress == null)
                return false;

            _context.Progresses.Remove(progress);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ChildProgressSummaryDto> GetChildProgressSummaryAsync(int childId)
        {
            var child = await _context.Users.FindAsync(childId);
            if (child == null)
                throw new InvalidOperationException("Child not found");

            var progressList = await GetProgressByUserIdAsync(childId);
            var badges = await GetUserBadgesAsync(childId);

            var summary = new ChildProgressSummaryDto
            {
                ChildId = childId,
                ChildName = $"{child.FirstName} {child.LastName}",
                TotalModules = progressList.Count,
                CompletedModules = progressList.Count(p => p.Status == ProgressStatus.Completed),
                InProgressModules = progressList.Count(p => p.Status == ProgressStatus.InProgress),
                AverageScore = progressList.Any() ? progressList.Average(p => p.Percentage) : 0,
                TotalTimeSpentMinutes = progressList.Sum(p => p.TimeSpentMinutes),
                TotalBadges = badges.Count,
                RecentProgress = progressList.Take(5).ToList(),
                RecentBadges = badges.Take(3).ToList()
            };

            return summary;
        }

        public async Task<List<ChildProgressSummaryDto>> GetChildrenProgressSummaryAsync(int parentId)
        {
            var children = await _context.Users
                .Where(u => u.ParentId == parentId && u.Role == UserRole.Child)
                .ToListAsync();

            var summaries = new List<ChildProgressSummaryDto>();
            foreach (var child in children)
            {
                var summary = await GetChildProgressSummaryAsync(child.Id);
                summaries.Add(summary);
            }

            return summaries;
        }

        public async Task<List<BadgeDto>> GetUserBadgesAsync(int userId)
        {
            var userBadges = await _context.UserBadges
                .Include(ub => ub.Badge)
                .Where(ub => ub.UserId == userId)
                .OrderByDescending(ub => ub.EarnedAt)
                .ToListAsync();

            return userBadges.Select(MapToBadgeDto).ToList();
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.RelatedUser)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(MapToNotificationDto).ToList();
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        private async Task CheckAndAwardBadgesAsync(int userId, Progress progress)
        {
            var user = await _context.Users
                .Include(u => u.UserBadges)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return;

            var badges = await _context.Badges.ToListAsync();
            var userProgress = await GetProgressByUserIdAsync(userId);

            foreach (var badge in badges)
            {
                if (user.UserBadges.Any(ub => ub.BadgeId == badge.Id))
                    continue; // Already earned

                bool shouldAward = false;

                switch (badge.Type)
                {
                    case BadgeType.FirstTime:
                        shouldAward = userProgress.Count(p => p.Status == ProgressStatus.Completed) == 1;
                        break;
                    case BadgeType.PerfectScore:
                        shouldAward = progress.Score == 100;
                        break;
                    case BadgeType.Speed:
                        shouldAward = progress.TimeSpentMinutes <= 20 && progress.Status == ProgressStatus.Completed;
                        break;
                    case BadgeType.Persistence:
                        shouldAward = userProgress.Count(p => p.Status == ProgressStatus.Completed) >= 5;
                        break;
                    case BadgeType.SubjectMaster:
                        var mathProgress = userProgress.Where(p => p.Subject == "Mathematics" && p.Status == ProgressStatus.Completed);
                        shouldAward = mathProgress.Count() >= 3 && mathProgress.Average(p => p.Percentage) >= 80;
                        break;
                }

                if (shouldAward)
                {
                    var userBadge = new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id,
                        ProgressId = progress.Id,
                        EarnedAt = DateTime.UtcNow,
                        IsNotified = false
                    };

                    _context.UserBadges.Add(userBadge);
                    await CreateNotificationAsync(userId, "Nouveau Badge !", 
                        $"Félicitations ! Vous avez gagné le badge '{badge.Name}'", 
                        NotificationType.BadgeEarned, NotificationPriority.High, null, badge.Id);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateNotificationAsync(int userId, string title, string message, 
            NotificationType type, NotificationPriority priority, int? progressId = null, int? badgeId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                Priority = priority,
                RelatedProgressId = progressId,
                RelatedBadgeId = badgeId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        private static ProgressDto MapToProgressDto(Progress progress)
        {
            return new ProgressDto
            {
                Id = progress.Id,
                UserId = progress.UserId,
                UserName = $"{progress.User.FirstName} {progress.User.LastName}",
                ModuleId = progress.ModuleId,
                ModuleTitle = progress.Module.Title,
                Subject = progress.Module.Subject.ToString(),
                Status = progress.Status,
                Score = progress.Score,
                MaxScore = progress.MaxScore,
                StartedAt = progress.StartedAt,
                CompletedAt = progress.CompletedAt,
                TimeSpentMinutes = progress.TimeSpentMinutes,
                Attempts = progress.Attempts,
                CreatedAt = progress.CreatedAt,
                UpdatedAt = progress.UpdatedAt
            };
        }

        private static BadgeDto MapToBadgeDto(UserBadge userBadge)
        {
            return new BadgeDto
            {
                Id = userBadge.Badge.Id,
                Name = userBadge.Badge.Name,
                Description = userBadge.Badge.Description,
                Type = userBadge.Badge.Type.ToString(),
                IconPath = userBadge.Badge.IconPath,
                Color = userBadge.Badge.Color,
                EarnedAt = userBadge.EarnedAt,
                IsNotified = userBadge.IsNotified
            };
        }

        private static NotificationDto MapToNotificationDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                Priority = notification.Priority.ToString(),
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                RelatedUserName = notification.RelatedUser != null ? 
                    $"{notification.RelatedUser.FirstName} {notification.RelatedUser.LastName}" : null
            };
        }
    }
} 