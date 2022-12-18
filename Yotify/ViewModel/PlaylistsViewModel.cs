using Yotify.Core;
using Yotify.Data.Model.Playlist;

namespace Yotify.ViewModel
{
    internal class PlaylistsViewModel: ObservableObject
    {
        public RelayCommand PlaylistsCommand { get; set; }

        public ObservableCollection<IPlaylist> Playlists { get; set; } 

        public PlaylistsViewModel()
        {
            PlaylistsCommand = new RelayCommand(o =>
            {
                Debug.WriteLine("fetch playlists...");

                if (Playlists == null)
                {
                    Playlists = new ObservableCollection<IPlaylist>();
                }

                Playlists.Add(new YoutubePlaylist("Playlist 1", "description 1", "http://playlist", "http://thumbnail"));
                Playlists.Add(new YoutubePlaylist("Playlist 2", "description 2", "http://playlist", "http://thumbnail"));
                Playlists.Add(new YoutubePlaylist("Playlist 3", "description 3", "http://playlist", "http://thumbnail"));
                Playlists.Add(new YoutubePlaylist("Playlist 4", "description 4", "http://playlist", "http://thumbnail"));
            });
        }
    }
}
