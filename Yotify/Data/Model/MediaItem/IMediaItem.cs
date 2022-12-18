namespace Yotify.Data.Model.MediaItem
{
    internal interface IMediaItem
    {
        string Name { get; set; }

        string Description { get; set; }

        string MediaImageURL { get; set; }

        string MediaURL { get; set; }
    }
}
