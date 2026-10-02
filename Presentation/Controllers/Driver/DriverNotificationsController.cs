using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/drivers/notifications")]
    [Authorize(Roles = "Driver")]
    public class DriverNotificationsController : ControllerBase
    {
        private readonly IUserNotificationService _notificationService;

        public DriverNotificationsController(IUserNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false)
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var notifications = await _notificationService.GetNotificationsAsync(userId, unreadOnly);

                return Ok(new
                {
                    message = "Daftar notifikasi berhasil diambil",
                    count = notifications.Count,
                    notifications
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil notifikasi", error = ex.Message });
            }
        }

        [HttpPatch("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var notification = await _notificationService.MarkAsReadAsync(userId, notificationId);

                return Ok(new
                {
                    message = "Notifikasi ditandai sudah dibaca",
                    data = notification
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat memperbarui notifikasi", error = ex.Message });
            }
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var updatedCount = await _notificationService.MarkAllAsReadAsync(userId);

                return Ok(new
                {
                    message = "Semua notifikasi ditandai sudah dibaca",
                    data = new { updatedCount }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat memperbarui notifikasi", error = ex.Message });
            }
        }
    }
}
