using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby.Api
{
    public class EmbyTask
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public string IsHidden { get; set; }
    }
}
