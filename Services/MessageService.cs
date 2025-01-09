using Backend.Models;
using System.Text.Json;

namespace Backend.Services {
    public class MessageService {
        private readonly string _filePath = "messages.json";

        public async Task SendMessage(Message message) {
            var messages = await LoadMessagesFromJson();
            messages.Add(message);
            await SaveMessagesToJson(messages);
        }

        public async Task<List<Message>> GetUnreadMessages(string userId) {
            var allMessages = await LoadMessagesFromJson();
            var unreadMessages = allMessages.Where(m => m.to == userId).ToList();

            //Remove the messages after they have been read
            var messagesToKeep = allMessages.Where(m => m.to != userId).ToList();
            await SaveMessagesToJson(messagesToKeep);
            return unreadMessages;
        }


        private async Task<List<Message>> LoadMessagesFromJson() {
            try {
                if (File.Exists(_filePath)) {
                    using (FileStream openStream = File.OpenRead(_filePath)) {
                        return await JsonSerializer.DeserializeAsync<List<Message>>(openStream) ?? new List<Message>();
                    }
                } else {
                    return new List<Message>();
                }
            }
            catch (JsonException ex) {
                Console.WriteLine($"Error deserializing messages JSON: {ex.Message}");
                return new List<Message>();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error loading messages from JSON: {ex.Message}");
                return new List<Message>();
            }
        }

        private async Task SaveMessagesToJson(List<Message> messages) {
            try {
                using (FileStream createStream = File.Create(_filePath)) {
                    await JsonSerializer.SerializeAsync(createStream, messages);
                    await createStream.DisposeAsync();
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error saving messages to JSON: {ex.Message}");
            }
        }
    }
}
