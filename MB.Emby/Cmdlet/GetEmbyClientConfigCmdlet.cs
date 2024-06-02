using MB.Emby.Model;
using System.Management.Automation;

namespace MB.Emby.Cmdlet;

[Cmdlet(VerbsCommon.Get, "EmbyClientConfig")]
[OutputType(typeof(ClientConfig))]
public class GetEmbyClientConfigCmdlet : PSCmdlet
{
    protected override void ProcessRecord()
    {
        base.ProcessRecord();
        WriteObject(ClientConfig.GetConfig());
    }
}
