using EducProject.API.Models.DTOs;

namespace EducProject.API.Services
{
    public interface IProgressService
    {
        Task<ProgressDto?> GetProgressByIdAsync(int id);
        Task<List<ProgressDto>> GetProgressByUserIdAsync(int userId);
        Task<List<ProgressDto>> GetProgressByParentIdAsync(int parentId);
        Task<ProgressDto> CreateProgressAsync(CreateProgressDto createDto);
        Task<ProgressDto> UpdateProgressAsync(int id, UpdateProgressDto updateDto);
        Task<bool> DeleteProgressAsync(int id);
        Task<ChildProgressSummaryDto> GetChildProgressSummaryAsync(int childId);
        Task<List<ChildProgressSummaryDto>> GetChildrenProgressSummaryAsync(int parentId);
        Task<List<BadgeDto>> GetUserBadgesAsync(int userId);
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<bool> MarkNotificationAsReadAsync(int notificationId);
        Task<bool> MarkAllNotificationsAsReadAsync(int userId);
        Task<int> GetUnreadNotificationsCountAsync(int userId);
    }
} 