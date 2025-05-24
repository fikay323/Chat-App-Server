using Backend.Contracts;
using FirebaseAdmin.Messaging;

namespace Backend.Services
{
    public class PushNotificationService(IDeviceTokenService _tokenService): IPushNotificationService
    {
        public async Task SendNotificationToUser(string userId, string title, string body)
        {
            var token = await _tokenService.GetTokenAsync(userId);
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine($"No device token found for user: {userId}");
                return;
            }

            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High
                },
                Apns = new ApnsConfig
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "apns-priority", "10" }
                    }
                }
            };

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine($"Push notification sent: {response}");
        }
    }
}
