using EducProject.API.Models.DTOs;
using EducProject.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("children/{parentId}")]
        public async Task<ActionResult<List<UserResponseDto>>> GetChildrenByParent(int parentId)
        {
            var children = await _userService.GetChildrenByParentIdAsync(parentId);
            return Ok(children);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, [FromBody] UserRegistrationDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.UpdateUserAsync(id, updateDto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("check-username/{username}")]
        public async Task<ActionResult> CheckUsername(string username)
        {
            var exists = await _userService.UsernameExistsAsync(username);
            return Ok(new { exists });
        }

        [HttpGet("check-email/{email}")]
        public async Task<ActionResult> CheckEmail(string email)
        {
            var exists = await _userService.EmailExistsAsync(email);
            return Ok(new { exists });
        }
    }
} 