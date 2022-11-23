using System.Text.Json;

namespace Yotify.Authentication.Token
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
            string tokenJson = Properties.Settings.Default.AuthToken;

            if (tokenJson == "")
                return null;

            try
            {
                return JsonSerializer.Deserialize<AuthToken>(Properties.Settings.Default.AuthToken);
            } catch(JsonException)
            {
                return null;
            }
        }
    }
}
