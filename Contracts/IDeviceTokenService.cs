namespace Backend.Contracts
{
    public interface IDeviceTokenService
    {
        Task RegisterTokenAsync(string userId, string token);
        Task<string?> GetTokenAsync(string userId);
    }
}
