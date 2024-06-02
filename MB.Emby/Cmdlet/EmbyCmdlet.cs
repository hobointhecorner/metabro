using EmbyClient.Dotnet.Client;
using MB.Emby.Model;
using System.Management.Automation;

namespace MB.Emby.Cmdlet;

public class EmbyCmdlet : PSCmdlet
{
    [Parameter]
    public string? Url { get; set; } = null;

    [Parameter]
    public string? ApiKey { get; set; } = null;

    private protected ClientConfig? embyPref = ClientConfig.GetConfig();
    private protected Configuration? apiConfig;

    private protected bool TestClientConfig()
    {
        bool hasUrl = Url != null || (
            embyPref != null &&
            embyPref.Url != null
        );

        bool hasApiKey = ApiKey != null || (
            embyPref != null &&
            embyPref.ApiKey != null
        );

        return hasUrl && hasApiKey;
    }

    private protected void InitializeClient()
    {
        if (!TestClientConfig()) throw new Exception("'Url' and 'ApiKey' must both be provided as cmdlet parameters or configured using 'Set-EmbyPref'");

        if (Url != null) embyPref!.Url = Url;
        if (ApiKey != null) embyPref!.ApiKey = ApiKey;

        apiConfig = embyPref!.GetApiConfig();
    }
}
