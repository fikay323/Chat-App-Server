using Backend.Contracts;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs {
    public class ChatHub : Hub {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IPushNotificationService _pushNotificationService;
        private static Dictionary<string, string> _allConnections = new Dictionary<string, string>();

        public ChatHub(IMessageService messageService, IUserService userService, IPushNotificationService pushNotificationService) {
            _messageService = messageService;
            _userService = userService;
            _pushNotificationService = pushNotificationService;
        }

        public async Task Register(string username, string password) {
            try {
                var user = await _userService.Register(username, password);
                string connectionID = Context.ConnectionId;
                AddConnectionToStore(connectionID, user);
                await Clients.Caller.SendAsync("RegistrationSuccessful", user); // Send back user data
            }
            catch (Exception ex) {
                await Clients.Caller.SendAsync("RegistrationFailed", "User exists"); // Send error message
            }
        }
        
        public async Task Login(string username, string password) {
            try {
                var user = await _userService.Login(username, password);
                string connectionID = Context.ConnectionId;
                AddConnectionToStore(connectionID, user);
                await Clients.Caller.SendAsync("LoginSuccessful", user);
            } catch (Exception ex) {
                switch(ex.Message) {
                    case "user-not-found": 
                        await Clients.Caller.SendAsync("LoginFailed", "User not found, Pls sign up");
                        break;
                    case "incorrect-password":
                        await Clients.Caller.SendAsync("LoginFailed", "Invalid password.");
                        break;
                    default:
                        await Clients.Caller.SendAsync("LoginFailed", "An unexpected error occurred.");
                        break ;
                }
            }
        }

        public async Task SendMessage(Message messageToBeSent) {
            var message = new Message {
                sentBy = messageToBeSent.sentBy,
                senderName = await _userService.FindUserName(messageToBeSent.sentBy),
                to = messageToBeSent.to,
                content = messageToBeSent.content,
                messageID = Guid.NewGuid().ToString(),
                timestamp = DateTime.UtcNow
            };

            if (Clients != null && Clients.Client(messageToBeSent.to) != null) {
                List<string> connectionsToSendTo = GetAllUserInstances(messageToBeSent.to);
                await _pushNotificationService.SendNotificationToUser(message.to, message.senderName, message.content);
                if (connectionsToSendTo.Any()) {
                    await Clients.Clients(connectionsToSendTo).SendAsync("receive-message", message);
                } else {
                    // Handle the case where the user is not connected
                    await _messageService.SendMessage(message);
                }
            }
        }

        private static void AddConnectionToStore(string connectionID, User user) {
            _allConnections.Add(connectionID, user.UserID);
            Console.WriteLine(_allConnections.Count);
        }

        

        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            string disconnectedConnectionId = Context.ConnectionId;

            if (_allConnections.ContainsKey(disconnectedConnectionId)) {
                string disconnectedUserId = _allConnections[disconnectedConnectionId];
                _allConnections.Remove(disconnectedConnectionId);
                Console.WriteLine(_allConnections.Count);
            }

            await base.OnDisconnectedAsync(exception);
        }


        public async Task IsTyping(object isTyping, string recipientID, string senderID) {
            List<string> connectionsToSendTo = GetAllUserInstances(recipientID);

            if (connectionsToSendTo.Any()) {
                await Clients.Clients(connectionsToSendTo).SendAsync("typing", isTyping, senderID);
            }
        }

        public async Task GetUnreadMessages(string userID) {
            Console.WriteLine("Getting unread messages");
            var unreadMessages = await _messageService.GetUnreadMessages(userID);
            Console.WriteLine("{0} unread messages", unreadMessages.Count);
            if (unreadMessages.Any()) {
                await Clients.Caller.SendAsync("unread_messages", unreadMessages);
            }
        }
        
        public async Task SearchUsers(string keyword, string userID) {
            var users = await _userService.SearchUsers(keyword, userID);
            await Clients.Caller.SendAsync("search_produced", users);
        }

        public async Task JoinRoom(string roomID) {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomID);
        }

        private static List<string> GetAllUserInstances(string userID) {
            List<string> userInstances = _allConnections
                .Where(kvp => kvp.Value == userID)
                .Select(kvp => kvp.Key)
                .ToList();
            return userInstances;
        }
    }
}
