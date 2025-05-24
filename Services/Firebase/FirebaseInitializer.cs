using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Backend.Services.Firebase
{
    public class FirebaseInitializer
    {
        public static void InitializeFromEnv()
        {
            var json = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS_JSON");

            if (!string.IsNullOrEmpty(json))
            {
                var tempPath = Path.Combine(Path.GetTempPath(), "firebase-key.json");
                File.WriteAllText(tempPath, json, Encoding.UTF8);
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", tempPath);
            }
            else
            {
                throw new Exception("Firebase credentials not found in environment variables.");
            }
        }
    }
}
