using Microsoft.AspNetCore.Mvc;
using Backend.Models; // Replace with your namespace
using System.Text.Json;

namespace Backend.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase {
        private readonly List<User> _users;
        private readonly string _jsonFilePath;

        public UsersController(IConfiguration configuration) {
            _jsonFilePath = Path.Combine(AppContext.BaseDirectory, configuration.GetValue<string>("UsersFilePath") ?? "users.json");
            _users = LoadUsersFromJson(_jsonFilePath);

        }

        [HttpGet]
        public IActionResult GetUsers() {
            if (_users == null || _users.Count == 0) {
                return NotFound("No users found.");
            }

            return Ok(_users); // Use Ok() to return JSON
        }

        private List<User> LoadUsersFromJson(string filePath) {
            try {
                if (System.IO.File.Exists(filePath)) {
                    var json = System.IO.File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                } else {
                    // Create the file if it doesn't exist. Important for first run.
                    System.IO.File.WriteAllText(filePath, "[]");
                    return new List<User>();
                }
            }
            catch (JsonException ex) {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return new List<User>();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error loading users from JSON: {ex.Message}");
                return new List<User>();
            }
        }
    }
}