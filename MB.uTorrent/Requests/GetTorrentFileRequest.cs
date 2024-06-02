namespace MB.uTorrent.Requests;

internal class GetTorrentFileRequest : TorrentRequest
{
    internal GetTorrentFileRequest(string hash, Client client) : base(client)
    {
        SetParams(hash);
    }

    internal GetTorrentFileRequest(string url, string token, string hash) : base(url, token)
    {
        SetParams(hash);
    }

    private void SetParams(string hash)
    {
        this.parameters = new()
        {
            { "action", "getfiles" },
            { "hash", hash }
        };
    }
}
