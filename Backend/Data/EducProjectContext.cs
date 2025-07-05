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
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // Self-referencing relationship for Parent-Child
                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.Children)
                      .HasForeignKey(e => e.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
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
        }
    }
} 