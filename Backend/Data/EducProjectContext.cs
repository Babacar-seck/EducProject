using EducProject.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EducProject.API.Data
{
    public class EducProjectContext : DbContext
    {
        public EducProjectContext(DbContextOptions<EducProjectContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");

                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // Self-referencing relationship for Parent-Child
                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.Children)
                      .HasForeignKey(e => e.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Module configuration
            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Subject).IsRequired();
                entity.Property(e => e.AgeGroup).IsRequired();
                entity.Property(e => e.Level).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("datetime('now')");
            });

            // Progress configuration
            modelBuilder.Entity<Progress>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ModuleId).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("datetime('now')");

                entity.HasOne(e => e.User)
                      .WithMany(e => e.Progresses)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Module)
                      .WithMany(e => e.Progresses)
                      .HasForeignKey(e => e.ModuleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Badge configuration
            modelBuilder.Entity<Badge>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");

                entity.HasOne(e => e.Module)
                      .WithMany(e => e.Badges)
                      .HasForeignKey(e => e.ModuleId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // UserBadge configuration
            modelBuilder.Entity<UserBadge>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.BadgeId).IsRequired();
                entity.Property(e => e.EarnedAt).HasDefaultValueSql("datetime('now')");

                entity.HasOne(e => e.User)
                      .WithMany(e => e.UserBadges)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Badge)
                      .WithMany(e => e.UserBadges)
                      .HasForeignKey(e => e.BadgeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Progress)
                      .WithMany()
                      .HasForeignKey(e => e.ProgressId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Message).HasMaxLength(500);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");

                entity.HasOne(e => e.User)
                      .WithMany(e => e.Notifications)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.RelatedUser)
                      .WithMany()
                      .HasForeignKey(e => e.RelatedUserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.RelatedProgress)
                      .WithMany()
                      .HasForeignKey(e => e.RelatedProgressId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.RelatedBadge)
                      .WithMany()
                      .HasForeignKey(e => e.RelatedBadgeId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Create a default admin user
            var adminUser = new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@educproject.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "Admin",
                LastName = "User",
                DateOfBirth = new DateTime(1990, 1, 1),
                Age = 33,
                Role = UserRole.Admin,
                Level = UserLevel.Advanced,
                CreatedAt = DateTime.UtcNow
            };

            modelBuilder.Entity<User>().HasData(adminUser);

            // Seed modules
            var modules = new[]
            {
                new Module { Id = 1, Title = "Addition et Soustraction", Description = "Apprendre les bases des mathématiques", Subject = Subject.Mathematics, AgeGroup = 6, Level = ModuleLevel.Beginner, EstimatedDurationMinutes = 30, MaxScore = 100 },
                new Module { Id = 2, Title = "Multiplication", Description = "Découvrir la multiplication", Subject = Subject.Mathematics, AgeGroup = 8, Level = ModuleLevel.Intermediate, EstimatedDurationMinutes = 45, MaxScore = 100 },
                new Module { Id = 3, Title = "Les Animaux", Description = "Découvrir le monde animal", Subject = Subject.Science, AgeGroup = 7, Level = ModuleLevel.Beginner, EstimatedDurationMinutes = 25, MaxScore = 100 },
                new Module { Id = 4, Title = "Grammaire Française", Description = "Les bases de la grammaire", Subject = Subject.Language, AgeGroup = 9, Level = ModuleLevel.Intermediate, EstimatedDurationMinutes = 40, MaxScore = 100 },
                new Module { Id = 5, Title = "Histoire de France", Description = "Les grandes périodes historiques", Subject = Subject.History, AgeGroup = 10, Level = ModuleLevel.Beginner, EstimatedDurationMinutes = 35, MaxScore = 100 }
            };

            modelBuilder.Entity<Module>().HasData(modules);

            // Seed badges
            var badges = new[]
            {
                new Badge { Id = 1, Name = "Premier Pas", Description = "Premier module complété", Type = BadgeType.FirstTime, IconPath = "🎯", Color = "#FFD700", RequiredScore = 0 },
                new Badge { Id = 2, Name = "Parfait", Description = "Score parfait dans un module", Type = BadgeType.PerfectScore, IconPath = "⭐", Color = "#FF6B6B", RequiredScore = 100 },
                new Badge { Id = 3, Name = "Mathématicien", Description = "Expert en mathématiques", Type = BadgeType.SubjectMaster, IconPath = "🧮", Color = "#4ECDC4", RequiredScore = 0 },
                new Badge { Id = 4, Name = "Persévérant", Description = "A complété 5 modules", Type = BadgeType.Persistence, IconPath = "💪", Color = "#45B7D1", RequiredScore = 0 },
                new Badge { Id = 5, Name = "Rapide", Description = "Module complété en moins de 20 minutes", Type = BadgeType.Speed, IconPath = "⚡", Color = "#96CEB4", RequiredScore = 0 }
            };

            modelBuilder.Entity<Badge>().HasData(badges);
        }

        // Method to manually seed data if needed
        public void SeedInitialData()
        {
            if (!Users.Any(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@educproject.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FirstName = "Admin",
                    LastName = "User",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Age = 33,
                    Role = UserRole.Admin,
                    Level = UserLevel.Advanced,
                    CreatedAt = DateTime.UtcNow
                };

                Users.Add(adminUser);
                SaveChanges();
            }
        }
    }
} 