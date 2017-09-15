using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby.Api
{
    public class EmbySeries : EmbyItem
    {
        public string CommunityRating { get; set; }
        public string PremiereDate { get; set; }
        public string ProductionYear { get; set; }
        public float RunTimeTicks { get; set; }
        public string Status { get; set; }
    }
}
