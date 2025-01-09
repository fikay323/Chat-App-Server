using Backend.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Backend.Services {
    // Services/UserService.cs
    public class UserService {
        private readonly string _filePath = "users.json";

        public UserService() {
            //No longer using in memory list
        }

        public async Task<User> Register(string username, string password) {
            var users = await LoadUsersFromJson();

            if (users.Any(u => u.Username == username)) {
                throw new Exception("Username already exists.");
            }

            var user = new User {
                Username = username,
                Password = password,
                UserID = Guid.NewGuid().ToString()
            };

            users.Add(user);
            await SaveUsersToJson(users);
            return user;
        }

        public async Task<User> Login(string username, string password) {
            var users = await LoadUsersFromJson();
            var user = users.FirstOrDefault(u => u.Username == username);

            if (user == null) {
                throw new Exception("user-not-found"); // No user found with the given username
            } else if (user.Password != password) {
                throw new Exception("incorrect-password"); // Password does not match
            } else {
                return user;
            }

        }

        public async Task<List<object>> SearchUsers(string keyword, string currentUserId) {
            var users = await LoadUsersFromJson();
            return users
                .Where(u => u.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) && u.UserID != currentUserId)
                .Select(u => new { Username = u.Username, UserID = u.UserID })
                .Cast<object>() // Cast to object to avoid issues with anonymous types in serialization
                .ToList();
        }



        private async Task<List<User>> LoadUsersFromJson() {
            try {
                if (File.Exists(_filePath)) {
                    using (FileStream openStream = File.OpenRead(_filePath)) {
                        return await JsonSerializer.DeserializeAsync<List<User>>(openStream) ?? new List<User>();
                    }
                } else {
                    return new List<User>(); // Return empty list if file doesn't exist
                }
            }
            catch (JsonException ex) {
                Console.WriteLine($"Error deserializing users JSON: {ex.Message}");
                return new List<User>();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error loading users from JSON: {ex.Message}");
                return new List<User>();
            }
        }

        private async Task SaveUsersToJson(List<User> users) {
            try {
                using (FileStream createStream = File.Create(_filePath)) {
                    await JsonSerializer.SerializeAsync(createStream, users);
                    await createStream.DisposeAsync();
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error saving users to JSON: {ex.Message}");
            }
        }

        public async Task<string> FindUserName(string id) { 
            var users = await LoadUsersFromJson();
            var userFound = users.FirstOrDefault(u => u.UserID == id);
            if (userFound != null) {
                return userFound.Username;
            } else {
                return null;
            }
        }
    }

}
