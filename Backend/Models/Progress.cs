using System.ComponentModel.DataAnnotations;

namespace EducProject.API.Models
{
    public class Progress
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        
        public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
        
        public int Score { get; set; } = 0;
        
        public int MaxScore { get; set; } = 100;
        
        public DateTime? StartedAt { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        
        public int TimeSpentMinutes { get; set; } = 0;
        
        public int Attempts { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ProgressStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Failed = 3,
        Paused = 4
    }
} 