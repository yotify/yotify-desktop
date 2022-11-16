using System.Collections.Generic;
using Yotify.Data.Model.MediaItem;

namespace Yotify.Data.Model.Playlist
{
    internal interface IPlaylist
    {
        string Name { get; set; }

        string Description { get; set; }

        string PlaylistURL { get; set; }

        string ThumbnailURL { get; set; }

        List<IMediaItem> MediaItems { get; set; }
    }
}
