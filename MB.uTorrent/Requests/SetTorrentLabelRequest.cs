namespace MB.uTorrent.Requests;

internal class SetTorrentLabelRequest : TorrentRequest
{
    internal SetTorrentLabelRequest(string hash, string label, Client client) : base(client)
    {
        SetParams(hash, label);
    }

    internal SetTorrentLabelRequest(string url, string token, string hash, string label) : base(url, token)
    {
        SetParams(hash, label);
    }

    private void SetParams(string hash, string label)
    {
        this.parameters = new Dictionary<string, string>()
        {
            { "action", "setprops" },
            { "hash", hash },
            { "s", "label" },
            { "v", label }
        };
    }
}
