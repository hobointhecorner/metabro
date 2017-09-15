using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.uTorrent.Api
{
    public class Torrent
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string[] StatusTag { get; set; }
        public string Label { get; set; }
        public string Hash { get; set; }
        public string DateCompleted { get; set; }
        public string[] VideoFile { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
