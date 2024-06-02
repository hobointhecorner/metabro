using MB.Metadata.Text;

namespace MB.uTorrent;

public class TorrentInfo : TextMetadata
{
    public string Name { get; set; }
    public string Hash { get; set; }
    public string? Provider { get; set; }

    public TorrentInfo(Torrent torrent, string[]? seriesNames = null) : base(torrent.Name!, seriesNames)
    {
        Name = torrent.Name!;
        Hash = torrent.Hash;
        Provider = torrent.Label;
    }
}
