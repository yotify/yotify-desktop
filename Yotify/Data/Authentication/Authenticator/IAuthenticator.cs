using System.Threading.Tasks;
using Yotify.Data.Authentication.Token;

namespace Yotify.Data.Authentication.Authenticator
{
    interface IAuthenticator
    {
        public Task<AuthToken> Authenticate();

        public Task<AuthToken> RefreshToken(AuthToken token);
    }
}
