using EducProject.API.Data;
using EducProject.API.Models;
using EducProject.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EducProject.API.Services
{
    public class UserService : IUserService
    {
        private readonly EducProjectContext _context;

        public UserService(EducProjectContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Children)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user != null ? MapToUserResponseDto(user) : null;
        }

        public async Task<UserResponseDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.Children)
                .FirstOrDefaultAsync(u => u.Username == username);

            return user != null ? MapToUserResponseDto(user) : null;
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Children)
                .ToListAsync();

            return users.Select(MapToUserResponseDto).ToList();
        }

        public async Task<UserResponseDto> CreateUserAsync(UserRegistrationDto registrationDto)
        {
            // Check if username or email already exists
            if (await UsernameExistsAsync(registrationDto.Username))
                throw new InvalidOperationException("Username already exists");

            if (await EmailExistsAsync(registrationDto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password),
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                DateOfBirth = registrationDto.DateOfBirth,
                Age = CalculateAge(registrationDto.DateOfBirth),
                Role = registrationDto.Role,
                Level = registrationDto.Level,
                ParentId = registrationDto.ParentId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await GetUserByIdAsync(user.Id) ?? throw new InvalidOperationException("Failed to create user");
        }

        public async Task<UserResponseDto> CreateChildAsync(ChildRegistrationDto childDto)
        {
            // Verify parent exists
            var parent = await _context.Users.FindAsync(childDto.ParentId);
            if (parent == null)
                throw new InvalidOperationException("Parent not found");

            if (parent.Role != UserRole.Parent)
                throw new InvalidOperationException("Specified user is not a parent");

            // Check if username already exists
            if (await UsernameExistsAsync(childDto.Username))
                throw new InvalidOperationException("Username already exists");

            var child = new User
            {
                Username = childDto.Username,
                Email = $"{childDto.Username}@educproject.child", // Temporary email for children
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(childDto.Password),
                FirstName = childDto.FirstName,
                LastName = childDto.LastName,
                DateOfBirth = childDto.DateOfBirth,
                Age = CalculateAge(childDto.DateOfBirth),
                Role = UserRole.Child,
                Level = UserLevel.Beginner,
                ParentId = childDto.ParentId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(child);
            await _context.SaveChangesAsync();

            return await GetUserByIdAsync(child.Id) ?? throw new InvalidOperationException("Failed to create child");
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UserRegistrationDto updateDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Check if new username or email conflicts with other users
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => (u.Username == updateDto.Username || u.Email == updateDto.Email) && u.Id != id);

            if (existingUser != null)
                throw new InvalidOperationException("Username or email already exists");

            user.Username = updateDto.Username;
            user.Email = updateDto.Email;
            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.DateOfBirth = updateDto.DateOfBirth;
            user.Age = CalculateAge(updateDto.DateOfBirth);
            user.Role = updateDto.Role;
            user.Level = updateDto.Level;

            // Only update password if provided
            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
            }

            await _context.SaveChangesAsync();

            return await GetUserByIdAsync(user.Id) ?? throw new InvalidOperationException("Failed to update user");
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserResponseDto>> GetChildrenByParentIdAsync(int parentId)
        {
            var children = await _context.Users
                .Where(u => u.ParentId == parentId && u.Role == UserRole.Child)
                .ToListAsync();

            return children.Select(MapToUserResponseDto).ToList();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        private static UserResponseDto MapToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Role = user.Role,
                Level = user.Level,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                ParentId = user.ParentId,
                Children = user.Children.Select(MapToUserResponseDto).ToList()
            };
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
} 