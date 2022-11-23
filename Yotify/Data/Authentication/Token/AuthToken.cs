using System;

namespace Yotify.Data.Authentication.Token
{
    class AuthToken
    {
        public string TokenId { get; private set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string TokenType { get; private set; }
        public string[] Scopes { get; private set; }
        public DateTime ExpireTime { get; private set; }

        public AuthToken(
            string tokenId, 
            string accessToken, 
            string refreshToken, 
            string tokenType, 
            string[] scopes, 
            DateTime expireTime
        )
        {
            TokenId = tokenId;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            TokenType = tokenType;
            Scopes = scopes;
            ExpireTime = expireTime;
        }
    }
}
