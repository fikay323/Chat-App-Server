using Backend.Models;

namespace Backend.Contracts
{
    public interface IUserService
    {
        Task<User> Register(string username, string password);
        Task<User> Login(string username, string password);
        Task<List<object>> SearchUsers(string keyword, string currentUserId);
        Task<string> FindUserName(string id);
    }
}
