using Backend.Contracts;
using Backend.Hubs;
using Backend.Models;
using Backend.Services;
using Backend.Services.Firebase;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Load user data from JSON file
var users = LoadUsersFromJson("users.json");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.WithOrigins("https://chat-app-client-fikay323s-projects.vercel.app", "http://localhost:4200", "https://chat-app-client-git-main-fikay323s-projects.vercel.app", "https://chat-app-client-gamma-five.vercel.app", "https://chat-app-client-jq7r3vsj0-fikay323s-projects.vercel.app")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


// Register services
builder.Services.AddSingleton(users);
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IMessageService, MessageService>();

builder.Services.AddSingleton<IDeviceTokenService, DeviceTokenService>();
builder.Services.AddSingleton<IPushNotificationService, PushNotificationService>();


builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();

var firebaseKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "firebase-key.json");
FirebaseInitializer.InitializeFromEnv();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.MapHub<ChatHub>("/chatHub");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

List<User> LoadUsersFromJson(string filePath) {
    try {
        if (File.Exists(filePath)) {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        } else {
            // Create the file if it doesn't exist. Important for first run.
            File.WriteAllText(filePath, "[]");
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