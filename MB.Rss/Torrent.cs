namespace MB.Rss;

public class Torrent
{
    public string Title { get; set; }
    public DateTime PubDate { get; set; }
    public string Provider { get; set; }
    public string Link { get; set; }

    public Torrent()
    {
        Title = string.Empty;
        PubDate = DateTime.MinValue;
        Provider = string.Empty;
        Link = string.Empty;
    }

    public Torrent(string title) : this()
    {
        Title = title;
    }

    public Torrent(string title, string link, DateTime pubDate, string provider) : this(title)
    {
        PubDate = pubDate;
        Provider = provider;
        Link = link;
    }

    public override string ToString()
    {
        return Title;
    }
}
