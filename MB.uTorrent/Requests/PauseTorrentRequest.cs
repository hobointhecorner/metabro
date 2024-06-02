namespace MB.uTorrent.Requests;

internal class PauseTorrentRequest : TorrentRequest
{
    internal PauseTorrentRequest(string hash, Client client) : base(client)
    {
        SetParams(hash);
    }

    internal PauseTorrentRequest(string url, string token, string hash) : base(url, token)
    {
        SetParams(hash);
    }

    private void SetParams(string hash)
    {
        this.parameters = new Dictionary<string, string>()
        {
            { "action", "pause" },
            { "hash", hash }
        };
    }
}
