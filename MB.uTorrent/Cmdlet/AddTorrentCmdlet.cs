using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Add, nameof(Torrent))]
[OutputType(typeof(Torrent))]
public class AddTorrentCmdlet : UTorrentCmdlet
{
    [Parameter(Position = 0, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string[]? Link { get; set; }

    [Parameter]
    public string[]? SeriesName { get; set; } = null;

    [Parameter]
    [ValidateNotNullOrEmpty]
    public bool AddHistory { get; set; } = true;

    [Parameter]
    [ValidateNotNullOrEmpty]
    public bool Wait { get; set; } = true;

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    List<TorrentHistory>? history = TorrentHistory.GetTorrentHistory();

    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        InitializeClient();

        if (AddHistory || PassThru) Wait = true;
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        foreach (var link in Link!)
        {
            bool first = true;
            var torrentList = Torrent.GetTorrent(client);
            var prevHash = from torrent in torrentList select torrent.Hash;
            int prevCount = prevHash.Count();

            do
            {
                if (first)
                {
                    Torrent.AddTorrent(link!);
                    first = false;
                }

                if (Wait)
                {
                    Thread.Sleep(1000);
                    torrentList = Torrent.GetTorrent(client);
                }
            }
            while (Wait && torrentList.Count == prevCount);

            if (Wait)
            {
                var newTorrents = from torrent in torrentList
                                  where !prevHash.Contains(torrent.Hash)
                                  select torrent;

                var newTorrent = newTorrents.First();

                if (AddHistory)
                {
                    var info = new TorrentInfo(newTorrent, SeriesName);
                    TorrentHistory.AddTorrentHistory(new TorrentHistory(info), client!.MaxHistoryItems);
                }

                if (PassThru) WriteObject(newTorrent);
            }
        }
    }
}
