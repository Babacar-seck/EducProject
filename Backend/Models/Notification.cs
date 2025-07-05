using System.ComponentModel.DataAnnotations;

namespace EducProject.API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public NotificationType Type { get; set; }
        
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        
        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ReadAt { get; set; }
        
        public int? RelatedUserId { get; set; } // For parent notifications about children
        public User? RelatedUser { get; set; }
        
        public int? RelatedProgressId { get; set; }
        public Progress? RelatedProgress { get; set; }
        
        public int? RelatedBadgeId { get; set; }
        public Badge? RelatedBadge { get; set; }
    }

    public enum NotificationType
    {
        ProgressUpdate = 1,
        BadgeEarned = 2,
        ModuleCompleted = 3,
        Achievement = 4,
        Reminder = 5,
        System = 6
    }

    public enum NotificationPriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Urgent = 4
    }
} 