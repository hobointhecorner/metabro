using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby.Api
{
    public class EmbyEpisode : EmbyItem
    {
        public string PremiereDate { get; set; }
        public string ProductionYear { get; set; }
        public int IndexNumber { get; set; }
        public int ParentIndexNumber { get; set; }
        public bool IsHD { get; set; }
        public string SeriesId { get; set; }
        public string SeriesName { get; set; }
    }

    public class EmbyEpisodeDetail : EmbyEpisode
    {

    }
}
