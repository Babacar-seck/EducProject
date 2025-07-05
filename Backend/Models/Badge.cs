using System.ComponentModel.DataAnnotations;

namespace EducProject.API.Models
{
    public class Badge
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public BadgeType Type { get; set; }
        
        public string IconPath { get; set; } = string.Empty;
        
        public string Color { get; set; } = "#FFD700"; // Gold default
        
        public int RequiredScore { get; set; } = 0;
        
        public int? ModuleId { get; set; }
        public Module? Module { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public List<UserBadge> UserBadges { get; set; } = new();
    }

    public enum BadgeType
    {
        Completion = 1,
        PerfectScore = 2,
        Speed = 3,
        Persistence = 4,
        SubjectMaster = 5,
        Streak = 6,
        FirstTime = 7,
        Special = 8
    }
} 