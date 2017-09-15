using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby.Api
{
    public class EmbyPlaylist : EmbyItem
    {

    }

    public class EmbyPlaylistItem : EmbyEpisode
    {
        public string PlaylistItemId { get; set; }
    }
}
