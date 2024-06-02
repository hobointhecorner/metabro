using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Remove, "TorrentPrivateTracker")]
public class RemoveTorrentPrivateTrackerCmdlet : PSCmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public ClientConfig ConfigObject { get; set; } = ClientConfig.GetConfig()!;

    [Parameter(Position = 0, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string? Tracker { get; set; }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        if (ConfigObject.PrivateTrackers != null && ConfigObject.PrivateTrackers.Contains(Tracker!))
        {
            ConfigObject.PrivateTrackers.Remove(Tracker!);
            ConfigObject.WriteConfig();
        }
    }
}
