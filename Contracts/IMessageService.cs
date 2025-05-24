using Backend.Models;

namespace Backend.Contracts
{
    public interface IMessageService
    {
        public Task SendMessage(Message message);
        Task<List<Message>> GetUnreadMessages(string userId);
    }
}
