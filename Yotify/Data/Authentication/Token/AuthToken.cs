using System;
using System.Collections.Generic;

namespace Yotify.Data.Authentication.Token
{
    class AuthToken
    {
        public string TokenId { get; private set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string TokenType { get; private set; }
        public string[] Scopes { get; private set; }
        public int ExpireTime { get; private set; } // TODO: calculate timestamp

        public AuthToken(
            string tokenId, 
            string accessToken, 
            string refreshToken, 
            string tokenType, 
            string[] scopes, 
            int expireTime
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
