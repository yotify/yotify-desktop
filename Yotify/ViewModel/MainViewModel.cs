using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yotify.Core;

namespace Yotify.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand PlaylistsViewCommand { get; set; }


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
        }
    }
}
