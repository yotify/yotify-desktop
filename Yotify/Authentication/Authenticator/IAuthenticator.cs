using System.Threading.Tasks;
using Yotify.Authentication.Token;

namespace Yotify.Authentication.Authenticator
{
    interface IAuthenticator
    {
        public Task Authenticate();

        public Task RefreshToken();
    }
}
