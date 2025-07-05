using EducProject.API.Models;
using EducProject.API.Models.DTOs;
using EducProject.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EducProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IProgressService _progressService;
        private readonly IUserService _userService;

        public TestController(IProgressService progressService, IUserService userService)
        {
            _progressService = progressService;
            _userService = userService;
        }

        [HttpPost("simulate-progress")]
        public async Task<ActionResult> SimulateProgress([FromBody] SimulateProgressRequest request)
        {
            try
            {
                // Create a test progress entry
                var createDto = new CreateProgressDto
                {
                    UserId = request.ChildId,
                    ModuleId = request.ModuleId,
                    Status = request.Status,
                    Score = request.Score,
                    TimeSpentMinutes = request.TimeSpentMinutes
                };

                var progress = await _progressService.CreateProgressAsync(createDto);

                // If completed, update with completion data
                if (request.Status == ProgressStatus.Completed)
                {
                    var updateDto = new UpdateProgressDto
                    {
                        Status = ProgressStatus.Completed,
                        Score = request.Score,
                        TimeSpentMinutes = request.TimeSpentMinutes,
                        Attempts = request.Attempts
                    };

                    await _progressService.UpdateProgressAsync(progress.Id, updateDto);
                }

                return Ok(new { message = "Progression simulée avec succès", progressId = progress.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("test-data")]
        public async Task<ActionResult> GetTestData()
        {
            try
            {
                // Get all users
                var users = await _userService.GetAllUsersAsync();
                var parents = users.Where(u => u.Role == UserRole.Parent).ToList();
                var children = users.Where(u => u.Role == UserRole.Child).ToList();

                return Ok(new
                {
                    parents = parents.Select(p => new { p.Id, p.FirstName, p.LastName, p.Username }),
                    children = children.Select(c => new { c.Id, c.FirstName, c.LastName, c.Username, c.ParentId }),
                    modules = new[]
                    {
                        new { Id = 1, Title = "Addition et Soustraction", Subject = "Mathematics" },
                        new { Id = 2, Title = "Multiplication", Subject = "Mathematics" },
                        new { Id = 3, Title = "Les Animaux", Subject = "Science" },
                        new { Id = 4, Title = "Grammaire Française", Subject = "Language" },
                        new { Id = 5, Title = "Histoire de France", Subject = "History" }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class SimulateProgressRequest
    {
        public int ChildId { get; set; }
        public int ModuleId { get; set; }
        public ProgressStatus Status { get; set; }
        public int Score { get; set; }
        public int TimeSpentMinutes { get; set; }
        public int Attempts { get; set; } = 1;
    }
} 