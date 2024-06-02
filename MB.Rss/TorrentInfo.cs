using MB.Metadata.Text;

namespace MB.Rss;

public class TorrentInfo : TextMetadata
{
    public string Provider { get; set; }

    public string Link { get; set; }

    public TorrentInfo(Torrent torrent, string[]? seriesNames = null) : base(torrent.Title, seriesNames)
    {
        this.Provider = torrent.Provider;
        this.Link = torrent.Link;
    }
}
