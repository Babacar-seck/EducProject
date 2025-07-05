using System.ComponentModel.DataAnnotations;

namespace EducProject.API.Models
{
    public class UserBadge
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        public int BadgeId { get; set; }
        public Badge Badge { get; set; } = null!;
        
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
        
        public int? ProgressId { get; set; }
        public Progress? Progress { get; set; }
        
        public bool IsNotified { get; set; } = false;
    }
} 