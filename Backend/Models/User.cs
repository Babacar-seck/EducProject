using System.ComponentModel.DataAnnotations;

namespace EducProject.API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public int Age { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public UserLevel Level { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public int? ParentId { get; set; }
        public User? Parent { get; set; }
        public List<User> Children { get; set; } = new();
        
        // Progress tracking
        public List<Progress> Progresses { get; set; } = new();
        public List<UserBadge> UserBadges { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
    }

    public enum UserRole
    {
        Child = 1,
        Parent = 2,
        Teacher = 3,
        Admin = 4
    }

    public enum UserLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
} 