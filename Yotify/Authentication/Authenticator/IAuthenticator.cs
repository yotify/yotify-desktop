using System.Threading.Tasks;
using Yotify.Authentication.Token;

namespace Yotify.Authentication.Authenticator
{
    interface IAuthenticator
    {
        public Task<AuthToken> Authenticate();

        public Task<AuthToken> RefreshToken(AuthToken token);
    }
}
