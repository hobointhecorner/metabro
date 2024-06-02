using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Get, nameof(Torrent))]
[OutputType(typeof(Torrent))]
public class GetTorrentCmdlet : UTorrentCmdlet
{
    [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
    public string[]? Hash { get; set; }

    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        InitializeClient();
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        List<Torrent> torrentList = new();

        if (Hash == null)
        {
            torrentList = Torrent.GetTorrent(client);
        }
        else
        {
            foreach (string hash in Hash)
            {
                var torrent = Torrent.GetTorrent(hash, client);

                if (torrent != null) torrentList.Add(torrent);
                else                 WriteWarning($"No torrent found for hash {hash}");
            }            
        }

        foreach (Torrent torrent in torrentList)
        {
            WriteObject(torrent);
        }
    }
}
