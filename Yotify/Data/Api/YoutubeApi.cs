using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Yotify.Data.Model.Playlist;

namespace Yotify.Data.Api
{
    internal class YoutubeApi
    {
        public HttpClient? Client { get; private set; }

        public async Task<string> Search()
        {
            string response = await GetClient().GetStringAsync("search");

            return response;
        }

        public async Task<List<IPlaylist>> GetPlaylistsForCurrentUser()
        {
            throw new NotImplementedException();
        }

        public Task<List<IPlaylist>> GetPlaylistsForUser(string userId)
        {
            throw new NotImplementedException();
        }

        private HttpClient GetClient()
        {
            if (Client != null)
            {
                return Client;
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://youtube.googleapis.com/youtube/v3/");
            client.Timeout = new TimeSpan(0, 0, 0, 5);

            Client = client;

            return client;
        }
    }
}
