using EducProject.API.Models.DTOs;
using EducProject.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EducProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResponse = await _authService.LoginAsync(loginDto);
            if (authResponse == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(authResponse);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.CreateUserAsync(registrationDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register-child")]
        public async Task<ActionResult<UserResponseDto>> RegisterChild([FromBody] ChildRegistrationDto childDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var child = await _userService.CreateChildAsync(childDto);
                return CreatedAtAction(nameof(GetUser), new { id = child.Id }, child);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("validate")]
        public async Task<ActionResult> ValidateToken([FromHeader(Name = "Authorization")] string? authorization)
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                return Unauthorized();

            var token = authorization.Substring("Bearer ".Length);
            var isValid = await _authService.ValidateTokenAsync(token);

            return isValid ? Ok() : Unauthorized();
        }
    }
} 