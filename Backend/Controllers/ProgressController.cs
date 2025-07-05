using EducProject.API.Models.DTOs;
using EducProject.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ProgressDto>>> GetUserProgress(int userId)
        {
            var progress = await _progressService.GetProgressByUserIdAsync(userId);
            return Ok(progress);
        }

        [HttpGet("parent/{parentId}")]
        public async Task<ActionResult<List<ProgressDto>>> GetParentChildrenProgress(int parentId)
        {
            var progress = await _progressService.GetProgressByParentIdAsync(parentId);
            return Ok(progress);
        }

        [HttpGet("child-summary/{childId}")]
        public async Task<ActionResult<ChildProgressSummaryDto>> GetChildProgressSummary(int childId)
        {
            try
            {
                var summary = await _progressService.GetChildProgressSummaryAsync(childId);
                return Ok(summary);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("children-summary/{parentId}")]
        public async Task<ActionResult<List<ChildProgressSummaryDto>>> GetChildrenProgressSummary(int parentId)
        {
            var summaries = await _progressService.GetChildrenProgressSummaryAsync(parentId);
            return Ok(summaries);
        }

        [HttpPost]
        public async Task<ActionResult<ProgressDto>> CreateProgress([FromBody] CreateProgressDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var progress = await _progressService.CreateProgressAsync(createDto);
                return CreatedAtAction(nameof(GetProgressById), new { id = progress.Id }, progress);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProgressDto>> UpdateProgress(int id, [FromBody] UpdateProgressDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var progress = await _progressService.UpdateProgressAsync(id, updateDto);
                return Ok(progress);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProgressDto>> GetProgressById(int id)
        {
            var progress = await _progressService.GetProgressByIdAsync(id);
            if (progress == null)
                return NotFound();

            return Ok(progress);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProgress(int id)
        {
            var deleted = await _progressService.DeleteProgressAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("badges/{userId}")]
        public async Task<ActionResult<List<BadgeDto>>> GetUserBadges(int userId)
        {
            var badges = await _progressService.GetUserBadgesAsync(userId);
            return Ok(badges);
        }

        [HttpGet("notifications/{userId}")]
        public async Task<ActionResult<List<NotificationDto>>> GetUserNotifications(int userId)
        {
            var notifications = await _progressService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpPut("notifications/{notificationId}/read")]
        public async Task<ActionResult> MarkNotificationAsRead(int notificationId)
        {
            var success = await _progressService.MarkNotificationAsReadAsync(notificationId);
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPut("notifications/{userId}/read-all")]
        public async Task<ActionResult> MarkAllNotificationsAsRead(int userId)
        {
            var success = await _progressService.MarkAllNotificationsAsReadAsync(userId);
            return Ok(new { success });
        }

        [HttpGet("notifications/{userId}/unread-count")]
        public async Task<ActionResult<int>> GetUnreadNotificationsCount(int userId)
        {
            var count = await _progressService.GetUnreadNotificationsCountAsync(userId);
            return Ok(count);
        }
    }
} 