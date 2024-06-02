using MB.File;
using System.Xml;

namespace MB.Rss;

public class Provider
{
    public string Name { get; set; }
    public string? Url { get; set; }
    public string? Type { get; set; }
    public int Priority { get; set; } = 100;
    public string RootKey { get; set; } = "//rss/channel/item";
    public string TitleAttr { get; set; } = "title";
    public string LinkAttr { get; set; } = "link";
    public string PubdateAttr { get; set; } = "pubDate";

    internal static string subDirectory = "rss";
    internal static string fileName = "providers.json";


    public Provider()
    {
        Name = string.Empty;
        Url = string.Empty;
        Type = string.Empty;
    }

    public Provider(string name, string url, string type, int priority = 100)
    {
        Name = name;
        Url = url;
        Type = type;
        Priority = priority;
    }

    public override string ToString()
    {
        return Name;
    }

    XmlDocument GetFeed()
    {
        XmlReader reader = XmlReader.Create(Url!);
        XmlDocument doc = new XmlDocument();
        doc.Load(reader);

        return doc;
    }

    public static List<Provider> GetProvider()
    {
        return FileProvider.GetFile<List<Provider>>(fileName, subDirectory);
    }

    public static void WriteProvider(List<Provider> torrentProvider)
    {
        FileProvider.WriteFile(torrentProvider, fileName, subDirectory);
    }

    public List<Torrent> GetTorrents()
    {
        List<Torrent> output = new();

        if (Type!.ToLower() != "disabled")
        {
            XmlDocument doc = GetFeed();

            XmlNodeList nodeList = doc!.DocumentElement!.SelectNodes(RootKey)!;
            foreach (XmlNode node in nodeList!)
            {
                output.Add(new Torrent()
                {
                    Title = node!.SelectSingleNode(TitleAttr)!.InnerXml,
                    Link = node!.SelectSingleNode(LinkAttr)!.InnerXml,
                    PubDate = DateTime.Parse(node!.SelectSingleNode(PubdateAttr)!.InnerXml),
                    Provider = Name
                });
            }
        }

        return output;
    }
}
