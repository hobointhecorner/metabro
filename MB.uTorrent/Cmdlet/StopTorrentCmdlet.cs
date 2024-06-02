using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsLifecycle.Stop, nameof(Torrent))]
public class StopTorrentCmdlet : UTorrentCmdlet
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
            torrent.Stop(client);
        }
    }
}
