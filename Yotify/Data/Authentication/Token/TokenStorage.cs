using System.Text.Json;

namespace Yotify.Data.Authentication.Token
{
    internal class TokenStorage
    {
        public static void StoreToken(AuthToken token)
        {
            Properties.Settings.Default.AuthToken = JsonSerializer.Serialize(token);
            Properties.Settings.Default.Save();
        }

        public static AuthToken? GetToken()
        {
            return JsonSerializer.Deserialize<AuthToken>(Properties.Settings.Default.AuthToken);
        }
    }
}
