using EducProject.API.Models.DTOs;

namespace EducProject.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<bool> ValidateTokenAsync(string token);
        Task<string> GenerateTokenAsync(User user);
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserResponseDto User { get; set; } = new();
        public DateTime ExpiresAt { get; set; }
    }
} 