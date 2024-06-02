using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Set, "TorrentClientConfig")]
public class SetClientConfigCmdlet : PSCmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public ClientConfig ConfigObject { get; set; } = ClientConfig.GetConfig()!;

    [Parameter]
    public string? Url { get; set; } = null;

    [Parameter]
    public PSCredential? Credential { get; set; } = null;

    [Parameter]
    [ValidateNotNullOrEmpty]
    public int? MaxHistoryItems { get; set; } = null;

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        if (Url != null) { ConfigObject.Url = Url; }
        if (Credential != null)
        {
            ConfigObject.Username = Credential.GetNetworkCredential().UserName;
            ConfigObject.Password = Credential.GetNetworkCredential().Password;
        }
        if (MaxHistoryItems != null) { ConfigObject.MaxHistoryItems = (int)MaxHistoryItems; }

        ConfigObject.WriteConfig();
    }
}
