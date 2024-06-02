using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsLifecycle.Start, nameof(Torrent))]
public class StartTorrentCmdlet : UTorrentCmdlet
{
    [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

        foreach (Torrent torrent in Torrent!)
        {
            torrent.Start(client);
        }
    }
}
