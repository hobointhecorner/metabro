using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Add, "TorrentPrivateTracker")]
public class AdduTorrentPrivateTrackerCmdlet : PSCmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public ClientConfig ConfigObject { get; set; } = ClientConfig.GetConfig()!;

    [Parameter(Position = 0, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string? Tracker { get; set; }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        if (ConfigObject.PrivateTrackers == null)
        {
            ConfigObject.PrivateTrackers = new List<string>() { Tracker! };
            ConfigObject.WriteConfig();
        }
        else if (!ConfigObject.PrivateTrackers.Contains(Tracker!))
        {
            ConfigObject.PrivateTrackers.Add(Tracker!);
            ConfigObject.WriteConfig();
        }
    }
}
