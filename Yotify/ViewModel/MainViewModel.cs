using System.Diagnostics;
using Yotify.Core;
using Yotify.Data.Authentication.Authenticator;

namespace Yotify.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand PlaylistsViewCommand { get; set; }

        public RelayCommand LoginCommand { get; set; }

        public HomeViewModel HomeVM { get; set; }

        public PlaylistsViewModel PlaylistsVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { 
                return _currentView;
            }
            set { 
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Debug.WriteLine("Init main viewmodel");

            HomeVM = new HomeViewModel();
            PlaylistsVM = new PlaylistsViewModel();

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                Debug.WriteLine("set home as current");
                CurrentView = HomeVM;
            });

            PlaylistsViewCommand = new RelayCommand(o =>
            {
                Debug.WriteLine("set playlists as current");
                CurrentView = PlaylistsVM;
            });

            // TODO: handle result async
            LoginCommand = new RelayCommand(o =>
            {
                Debug.WriteLine("Authentication started.");
                GoogleAuthenticator googleAuth = new GoogleAuthenticator();

                googleAuth.Authenticate();
            });
        }
    }
}
