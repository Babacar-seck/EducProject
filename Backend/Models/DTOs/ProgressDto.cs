using EducProject.API.Models;

namespace EducProject.API.Models.DTOs
{
    public class ProgressDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int ModuleId { get; set; }
        public string ModuleTitle { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public ProgressStatus Status { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public double Percentage => MaxScore > 0 ? (double)Score / MaxScore * 100 : 0;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TimeSpentMinutes { get; set; }
        public int Attempts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateProgressDto
    {
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
        public int Score { get; set; } = 0;
        public int TimeSpentMinutes { get; set; } = 0;
    }

    public class UpdateProgressDto
    {
        public ProgressStatus Status { get; set; }
        public int Score { get; set; }
        public int TimeSpentMinutes { get; set; }
        public int Attempts { get; set; }
    }

    public class ChildProgressSummaryDto
    {
        public int ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public int TotalModules { get; set; }
        public int CompletedModules { get; set; }
        public int InProgressModules { get; set; }
        public double AverageScore { get; set; }
        public int TotalTimeSpentMinutes { get; set; }
        public int TotalBadges { get; set; }
        public List<ProgressDto> RecentProgress { get; set; } = new();
        public List<BadgeDto> RecentBadges { get; set; } = new();
    }

    public class BadgeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTime EarnedAt { get; set; }
        public bool IsNotified { get; set; }
    }

    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? RelatedUserName { get; set; }
    }
} 