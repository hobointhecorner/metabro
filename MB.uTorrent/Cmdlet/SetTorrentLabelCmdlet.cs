using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Set, "TorrentLabel")]
public class SetTorrentLabelCmdlet : UTorrentCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
    [ValidateNotNullOrEmpty]
    public Torrent[]? Torrent { get; set; }

    [Parameter(Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string? Label { get; set; }

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
            torrent.SetLabel(Label!, client);
        }
    }
}
