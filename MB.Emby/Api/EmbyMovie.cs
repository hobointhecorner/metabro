using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby.Api
{
    public class EmbyMovie : EmbyItem
    {
        public string IsHD { get; set; }
        public string OfficialRating { get; set; }
        public string PremiereDate { get; set; }
        public string ProductionYear { get; set; }
        public long RunTimeTicks { get; set; } = 0;
    }
}
