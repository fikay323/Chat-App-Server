using System;
using System.Text.Json;
using Backend.Contracts;
using Backend.Entities;

namespace Backend.Services
{
    public class DeviceTokenService : IDeviceTokenService
    {
        private readonly string _filePath = "deviceTokens.json";


        public async Task<string?> GetTokenAsync(string userId)
        {
            var tokens = await LoadTokensFromJson();
            return tokens.FirstOrDefault(t => t.UserId == userId)?.Token;
        }

        public async Task RemoveTokenFromJson(string userId)
        {
            var tokens = await LoadTokensFromJson();
            var tokenToRemove = tokens.FirstOrDefault(t => t.UserId == userId);
            if (tokenToRemove != null)
            {
                tokens.Remove(tokenToRemove);
                await SaveTokensToJson(tokens);
            }
        }

        public async Task RegisterTokenAsync(string userId, string token)
        {
            var tokens = await LoadTokensFromJson();

            var existing = tokens.FirstOrDefault(t => t.UserId == userId);
            if (existing != null)
            {
                existing.Token = token;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                tokens.Add(new DeviceToken
                {
                    UserId = userId,
                    Token = token,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await SaveTokensToJson(tokens);
        }

        private async Task<List<DeviceToken>> LoadTokensFromJson()
        {
            try
            {
                if(File.Exists(_filePath))
                {
                    using var openStream = File.OpenRead(_filePath);
                    return await JsonSerializer.DeserializeAsync<List<DeviceToken>>(openStream) ?? new List<DeviceToken>();
                } else
                {
                    return new List<DeviceToken>();
                }
            } catch { return new List<DeviceToken>(); }
        }

        private async Task SaveTokensToJson(List<DeviceToken> tokens)
        {
            using var saveStream = File.OpenWrite(_filePath);
            await JsonSerializer.SerializeAsync(saveStream, tokens);
            await saveStream.DisposeAsync();
        }
    }
}
