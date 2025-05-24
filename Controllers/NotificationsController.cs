using Backend.Contracts;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class NotificationsController(IDeviceTokenService deviceTokenService, IPushNotificationService pushNotificationService) : BaseApiController
    {
        private readonly IDeviceTokenService _deviceTokenService = deviceTokenService;
        private readonly IPushNotificationService _pushNotificationService = pushNotificationService;

        [HttpPost("register-token")]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterDeviceTokenDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest("Missing userId or token.");

            await _deviceTokenService.RegisterTokenAsync(dto.UserId, dto.Token);
            return Ok(new { message = "Token registered successfully" });
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendPush([FromBody] SendPushRequest request)
        {
            await _pushNotificationService.SendNotificationToUser(request.UserId, request.Title, request.Body);
            return Ok(new { message = "Push sent (if token exists)" });
        }

        public class SendPushRequest
        {
            public string UserId { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
        }
    }
}