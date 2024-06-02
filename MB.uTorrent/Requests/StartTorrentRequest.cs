namespace MB.uTorrent.Requests;

internal class StartTorrentRequest : TorrentRequest
{
    internal StartTorrentRequest(string hash, Client client) : base(client)
    {
        SetParams(hash);
    }

    internal StartTorrentRequest(string url, string token, string hash) : base(url, token)
    {
        SetParams(hash);
    }

    private void SetParams(string hash)
    {
        this.parameters = new Dictionary<string, string>()
        {
            { "action", "start" },
            { "hash", hash }
        };
    }
}
