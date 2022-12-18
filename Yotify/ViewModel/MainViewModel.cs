using System;
using System.Diagnostics;
using System.Text.Json;
using Yotify.Authentication.Authenticator;
using Yotify.Authentication.Token;
using Yotify.Core;

namespace Yotify.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand PlaylistsViewCommand { get; set; }

        public RelayCommand LoginCommand { get; set; }

        public HomeViewModel HomeVM { get; set; }

        public PlaylistsViewModel PlaylistsVM { get; set; }

        private object? _currentView;

        public object? CurrentView
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

            LoginCommand = new RelayCommand(async o =>
            {
                IAuthenticator authenticator = new GoogleAuthenticator();

                AuthToken? token = TokenStorage.GetToken();

                if (token == null)
                {
                    Debug.WriteLine("No token found. Starting authentication...");
                    await authenticator.Authenticate(); // TODO: handle exception

                    Debug.WriteLine(String.Format("Saved token. Current Token content: {0}", JsonSerializer.Serialize(TokenStorage.GetToken())));

                    return;
                }

                await authenticator.RefreshToken();
                // TODO:    if refresh token generation fails -> start auth code flow
                // TODO:    handle exceptions
                Debug.WriteLine(String.Format("Saved token. Current Token content: {0}", JsonSerializer.Serialize(TokenStorage.GetToken())));
            });
        }
    }
}
