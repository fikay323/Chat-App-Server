using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Backend.Services.Firebase
{
    public class FirebaseInitializer
    {
        private static bool _initialized = false;

        public static void Initialize(string credentialsPath)
        {
            if (_initialized) return;

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(credentialsPath)
            });

            _initialized = true;
        }
    }
}
