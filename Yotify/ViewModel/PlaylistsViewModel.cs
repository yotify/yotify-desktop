using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotify.Core;
using Yotify.Data.Model.Playlist;

namespace Yotify.ViewModel
{
    internal class PlaylistsViewModel: ObservableObject
    {
        public List<IPlaylist> Playlists { get; set; }

        public PlaylistsViewModel()
        {
            Playlists = new List<IPlaylist>
            {
                new YoutubePlaylist("Playlist 1", "Very nice music", "https://3b34b435b345b.test", ""),
                new YoutubePlaylist("Playlist 2", "Very nice music", "https://3b34b435b345b.test", ""),
                new YoutubePlaylist("Playlist 3", "Very nice music", "https://3b34b435b345b.test", ""),
                new YoutubePlaylist("Playlist 4", "Very nice music", "https://3b34b435b345b.test", ""),
            };
        }
    }
}
