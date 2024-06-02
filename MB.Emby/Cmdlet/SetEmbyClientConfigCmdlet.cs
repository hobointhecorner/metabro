using MB.Emby.Model;
using System.Management.Automation;

namespace MB.Emby.Cmdlet;

[Cmdlet(VerbsCommon.Set, "EmbyClientConfig")]
public class SetEmbyClientConfigCmdlet : PSCmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public ClientConfig ConfigObject { get; set; } = ClientConfig.GetConfig()!;

    [Parameter]
    public string? Url { get; set; } = null;

    [Parameter]
    public string? ApiKey { get; set; } = null;

    [Parameter]
    public string? DefaultUserId { get; set; } = null;

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        if (Url != null) ConfigObject.Url = Url;
        if (ApiKey != null) ConfigObject.ApiKey = ApiKey;
        if (DefaultUserId != null) ConfigObject.DefaultUserId = DefaultUserId;

        ConfigObject.WriteConfig();
    }
}
