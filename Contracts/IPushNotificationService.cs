namespace Backend.Contracts
{
    public interface IPushNotificationService
    {
        Task SendNotificationToUser(string userId, string title, string body);
    }
}
