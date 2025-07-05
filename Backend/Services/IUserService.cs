using EducProject.API.Models;
using EducProject.API.Models.DTOs;

namespace EducProject.API.Services
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto?> GetUserByUsernameAsync(string username);
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> CreateUserAsync(UserRegistrationDto registrationDto);
        Task<UserResponseDto> CreateChildAsync(ChildRegistrationDto childDto);
        Task<UserResponseDto> UpdateUserAsync(int id, UserRegistrationDto updateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<List<UserResponseDto>> GetChildrenByParentIdAsync(int parentId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
} 