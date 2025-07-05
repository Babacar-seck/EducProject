using System.ComponentModel.DataAnnotations;

namespace EducProject.API.Models
{
    public class Module
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public Subject Subject { get; set; }
        
        [Required]
        public int AgeGroup { get; set; } // 6-8, 9-10, 11-12
        
        [Required]
        public ModuleLevel Level { get; set; }
        
        public int EstimatedDurationMinutes { get; set; } = 30;
        
        public int MaxScore { get; set; } = 100;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public List<Progress> Progresses { get; set; } = new();
        public List<Badge> Badges { get; set; } = new();
    }

    public enum Subject
    {
        Mathematics = 1,
        Science = 2,
        Language = 3,
        History = 4,
        Geography = 5,
        Arts = 6,
        PhysicalEducation = 7
    }

    public enum ModuleLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
} 