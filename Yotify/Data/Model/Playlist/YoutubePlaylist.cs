using System.Collections.Generic;
using Yotify.Data.Model.MediaItem;

namespace Yotify.Data.Model.Playlist
{
    internal class YoutubePlaylist : IPlaylist
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PlaylistURL { get; set; }
        public string ThumbnailURL { get; set; }
        public List<IMediaItem> MediaItems { get; set; }

        public YoutubePlaylist(string name, string description, string playlistURL, string thumbnailURL, List<IMediaItem>? mediaItems = null)
        {
            Name = name;
            Description = description;
            PlaylistURL = playlistURL;
            ThumbnailURL = thumbnailURL;
            MediaItems = mediaItems ?? new List<IMediaItem>();
        }

        public void AddMediaItem(IMediaItem mediaItem)
        {
            MediaItems.Add(mediaItem);
        }
    }
}
