using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;
using Yotify.Data.Authentication.Authenticator.Exception;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Yotify.Data.Authentication.Token;
using System.Configuration;

namespace Yotify.Data.Authentication.Authenticator
{
    class GoogleAuthenticator : IAuthenticator
    {
        private readonly string clientId = "";
        private readonly string clientSecret = "";
        private const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        private const string scope = "https://www.googleapis.com/auth/youtube profile";

        public GoogleAuthenticator()
        {
            clientId = ConfigurationManager.AppSettings["google_client_id"] ?? "";
            clientSecret = ConfigurationManager.AppSettings["google_client_secret"] ?? "";
        }

        public async Task<AuthToken> Authenticate() // TODO: return Token Object
        {
            string codeVerifier = GenerateRandomUrlEncodedBase64(32);
            string codeChallenge = ConvertToUrlEncodedBase64(Sha256(codeVerifier));
            string state = GenerateRandomUrlEncodedBase64(32);
            const string codeChallengeMethod = "S256";

            string redirectUri = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());

            HttpListener http = new();
            http.Prefixes.Add(redirectUri);
            Debug.WriteLine("Listening..");
            http.Start();

            string queryString = BuildQueryString(new Dictionary<string, string>()
            {
                { "response_type", "code" },
                { "redirect_uri", Uri.EscapeDataString(redirectUri) },
                { "scope", scope },
                { "client_id", clientId },
                { "state", state },
                { "code_challenge", codeChallenge },
                { "code_challenge_method", codeChallengeMethod }
            });

            string authorizationRequest = authorizationEndpoint + queryString;

            Process.Start(new ProcessStartInfo { FileName = authorizationRequest, UseShellExecute = true });


            var context = await http.GetContextAsync();

            var response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            if (ValidateContext(context) == false)
            {
                throw new AuthenticationException("Auth failed: invalid context");
            }

            try
            {
                string code = ExtractCodeFromContext(state, context);

                try
                {
                    return await ExchangeCodeForTokens(code, codeVerifier, redirectUri);
                }
                catch (InvalidResponseException ex)
                {
                    throw new AuthenticationException("Auth failed: invalid response", ex);
                }
            }
            catch (InvalidStateException ex)
            {
                throw new AuthenticationException("Auth failed: invalid state", ex);
            }
            catch (InvalidCodeException ex)
            {
                throw new AuthenticationException("Auth failed: invalid code", ex);
            }
        }

        public async Task<AuthToken> RefreshToken(AuthToken token) // TODO: better exceptions
        {
            string redirectUri = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());

            HttpListener http = new();
            http.Prefixes.Add(redirectUri);
            Debug.WriteLine("Listening..");
            http.Start();

            AuthToken? currentToken = TokenStorage.GetToken();

            if (currentToken == null)
                throw new AuthenticationException("Cannot get refreshed token for null");

            string queryString = BuildQueryString(new Dictionary<string, string>()
            {
                { "refresh_token", currentToken.RefreshToken },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "refresh_token" },
            });

            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            byte[] _byteVersion = Encoding.ASCII.GetBytes(queryString[1..]);
            tokenRequest.ContentLength = _byteVersion.Length;

            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using StreamReader streamReader = new(tokenResponse.GetResponseStream());
                string responseText = await streamReader.ReadToEndAsync();
                Debug.WriteLine(responseText);

                Dictionary<string, string> tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                if (tokenData == null)
                {
                    throw new InvalidResponseException("Invalid response json data");
                }

                if (!tokenData.ContainsKey("refresh_token"))
                    tokenData.Add("refresh_token", token.RefreshToken);

                return new AuthToken(
                    tokenData["id_token"],
                    tokenData["access_token"],
                    tokenData["refresh_token"],
                    tokenData["token_type"],
                    tokenData["scope"].Split(" "),
                    Convert.ToInt32(tokenData["expires_in"]) // TODO: ParseInt maybe
                );
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    throw new InvalidResponseException(String.Format("Invalid reponse status ({0})", ex.Status.ToString()), ex);
                }

                throw new InvalidResponseException("Invalid response");
            }
        }

        private int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }

        private string ExtractCodeFromContext(string state, HttpListenerContext context)
        {
            var code = context.Request.QueryString.Get("code");
            var receivedState = context.Request.QueryString.Get("state");

            if (receivedState != state)
                throw new InvalidStateException(String.Format("Invalid state in received request. ({0})", receivedState));

            if (code == null)
                throw new InvalidCodeException("Code not found in response");

            return code;
        }

        private bool ValidateContext(HttpListenerContext context)
        {
            if (context.Request.QueryString.Get("error") != null)
            {
                Debug.WriteLine("Auth errror: " + context.Request.QueryString.Get("error"));
                return false;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                Debug.WriteLine("Invalid auth response" + context.Request.QueryString);
                return false;
            }

            return true;
        }

        private string BuildQueryString(Dictionary<string, string> queryParams)
        {
            string queryString = "";

            bool first = true; // TODO: refactor
            foreach (KeyValuePair<string, string> param in queryParams)
            {
                if (first == true)
                {
                    queryString += $"?{param.Key}={param.Value}";
                    first = false;
                    continue;
                }

                queryString += $"&{param.Key}={param.Value}";
            }

            return queryString;
        }

        private string GenerateRandomUrlEncodedBase64(uint length)
        {
            byte[] bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);

            return ConvertToUrlEncodedBase64(bytes);
        }

        private string ConvertToUrlEncodedBase64(byte[] buffer)
        {
            return (new StringBuilder(Convert.ToBase64String(buffer)))
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", string.Empty)
                .ToString();
        }

        private byte[] Sha256(string inputString)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputString);

            return SHA256.Create().ComputeHash(bytes);
        }

        private async Task<AuthToken> ExchangeCodeForTokens(string code, string codeVerifier, string redirectURI)
        {
            Debug.WriteLine("Exchanging code with tokens");

            string queryString = BuildQueryString(new Dictionary<string, string>()
            {
                { "code", code },
                { "redirect_uri", Uri.EscapeDataString(redirectURI) },
                { "client_id", clientId },
                { "code_verifier", codeVerifier },
                { "client_secret", clientSecret },
                { "scope", "" },
                { "grant_type", "authorization_code" },
            });

            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            byte[] _byteVersion = Encoding.ASCII.GetBytes(queryString[1..]);
            tokenRequest.ContentLength = _byteVersion.Length;

            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using StreamReader streamReader = new(tokenResponse.GetResponseStream());
                string responseText = await streamReader.ReadToEndAsync();
                Debug.WriteLine(responseText);

                Dictionary<string, string> tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                if (tokenData == null)
                {
                    throw new InvalidResponseException("Invalid response json data");
                }

                return new AuthToken(
                    tokenData["id_token"],
                    tokenData["access_token"],
                    tokenData["refresh_token"],
                    tokenData["token_type"],
                    tokenData["scope"].Split(" "),
                    Convert.ToInt32(tokenData["expires_in"]) // TODO: ParseInt maybe
                );
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    throw new InvalidResponseException(String.Format("Invalid reponse status ({0})", ex.Status.ToString()), ex);
                }

                throw new InvalidResponseException("Invalid response");
            }
        }
    }
}
