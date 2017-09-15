using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MB.RSS
{
    public class RssClient
    {
        public static XmlDocument GetRssFeed(string Url)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Url);
            return doc;
        }
    }
}
