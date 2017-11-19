using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MB.RSS
{
    public class RssClient
    {
        public static XmlDocument GetRssFeed(string Url)
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.UserAgent = "Metabro";

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                return doc;
            }
        }
    }
}
