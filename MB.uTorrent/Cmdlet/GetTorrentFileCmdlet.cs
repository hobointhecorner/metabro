using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Get, nameof(TorrentFile))]
[OutputType(typeof(TorrentFile))]
public class GetTorrentFileCmdlet : UTorrentCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
    [ValidateNotNullOrEmpty]
    public Torrent[]? Torrent { get; set; }

    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        InitializeClient();
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        foreach (var torrent in Torrent!)
        {
            List<TorrentFile> fileList = TorrentFile.GetTorrentFile(torrent.Hash, client);

            foreach (TorrentFile torrentFile in fileList)
            {
                WriteObject(torrentFile);
            }
        }
    }
}
