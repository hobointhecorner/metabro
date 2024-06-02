namespace MB.uTorrent.Requests;

internal class StopTorrentRequest : TorrentRequest
{
    internal StopTorrentRequest(string hash, Client client) : base(client)
    {
        SetParams(hash);
    }

    internal StopTorrentRequest(string url, string token, string hash) : base(url, token)
    {
        SetParams(hash);
    }

    private void SetParams(string hash)
    {
        parameters = new()
        {
            { "action", "stop" },
            { "hash", hash }
        };
    }
}
