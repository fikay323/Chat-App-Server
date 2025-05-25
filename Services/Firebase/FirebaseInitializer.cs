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
                try
                {
                    var tempPath = Path.Combine(Path.GetTempPath(), "firebase-key-temp.json");
                    File.WriteAllText(tempPath, json, Encoding.UTF8);

                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", tempPath, EnvironmentVariableTarget.Process);

                    if (FirebaseApp.DefaultInstance == null)
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.GetApplicationDefault()
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Firebase initialization failed during FirebaseApp.Create(). Check JSON format and credentials.", ex);
                }
            }
            else
            {
                throw new Exception("Firebase credentials not found in environment variables. Please ensure GOOGLE_APPLICATION_CREDENTIALS_JSON is set correctly.");
            }
        }
    }
}
