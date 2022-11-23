using System;
using System.Diagnostics;
using System.Text.Json;
using Yotify.Core;
using Yotify.Data.Authentication.Authenticator;
using Yotify.Data.Authentication.Token;

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
                    token = await authenticator.Authenticate(); // TODO: handle exception
                    TokenStorage.StoreToken(token);
                    return;
                }
                token = await authenticator.RefreshToken(token); // TODO: pass reference & update
                // TODO:    if refresh token generation fails -> start auth code flow
                // TODO:    handle exceptions
                TokenStorage.StoreToken(token);

                Debug.WriteLine(String.Format("Saved token. Current Token content: {0}", JsonSerializer.Serialize(token)));
            });
        }
    }
}
