namespace Backend.Entities
{
    public class DeviceToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
