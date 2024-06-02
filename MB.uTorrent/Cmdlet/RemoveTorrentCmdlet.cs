using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Remove, nameof(Torrent))]
public class RemoveTorrentCmdlet : UTorrentCmdlet
{
    [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    [ValidateNotNullOrEmpty]
    public Torrent[]? Torrent { get; set; }

    [Parameter]
    public SwitchParameter RemoveData { get; set; }

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
            torrent.Remove(RemoveData, client);
        }
    }
}
