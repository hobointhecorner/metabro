using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

public class UTorrentCmdlet : PSCmdlet
{
    [Parameter]
    public string? Url { get; set; } = null;

    [Parameter]
    public PSCredential? Credential { get; set; } = null;

    private protected ClientConfig? torrentPref = ClientConfig.GetConfig();
    private protected Client? client;

    private protected bool TestClientConfig()
    {
        bool hasUrl = Url != null || (
            torrentPref != null &&
            torrentPref.Url != null
        );

        bool hasCredential = Credential != null || (
            torrentPref != null &&
            torrentPref.Username != null &&
            torrentPref.Password != null
        );

        return hasUrl && hasCredential;
    }

    private protected void InitializeClient()
    {
        if (!TestClientConfig())
        {
            throw new Exception("'Url', 'Username', and 'Password' must all be set using 'Set-TorrentPref'");
        }

        string url, username, password;

        if (Url != null) { url = Url; }
        else             { url = torrentPref!.Url!; }

        if (Credential != null)
        {
            username = Credential.GetNetworkCredential().UserName;
            password = Credential.GetNetworkCredential().Password;
        }
        else
        {
            username = torrentPref!.Username!;
            password = torrentPref!.Password!;
        }

        client = new Client(url, username, password);
    }
}
