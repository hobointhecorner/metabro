using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Get, "TorrentClientConfig")]
[OutputType(typeof(ClientConfig))]
public class GetClientConfigCmdlet : PSCmdlet
{
    protected override void ProcessRecord()
    {
        base.ProcessRecord();
        WriteObject(ClientConfig.GetConfig());
    }
}
