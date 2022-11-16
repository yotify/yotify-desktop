using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
