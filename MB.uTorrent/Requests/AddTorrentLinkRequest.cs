namespace MB.uTorrent.Requests;

internal class AddTorrentLinkRequest : TorrentRequest
{
    internal AddTorrentLinkRequest(string link, Client client) : base(client)
    {
        SetParams(link);
    }

    internal AddTorrentLinkRequest(string url, string token, string link) : base(url, token)
    {
        SetParams(link);
    }

    private void SetParams(string link)
    {
        this.parameters = new()
        {
            { "action", "add-url" },
            { "s", System.Web.HttpUtility.UrlEncode(link) }
        };
    }
}
