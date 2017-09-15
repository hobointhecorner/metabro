using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby.Api
{
    public class EmbyUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool HasPassword { get; set; }
        public string LastActivityDate { get; set; }
    }
}
