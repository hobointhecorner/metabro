namespace MB.uTorrent.Requests;

internal class RemoveTorrentRequest : TorrentRequest
{
    internal RemoveTorrentRequest(string hash, bool removeData, Client client) : base(client)
    {
        SetParams(hash, removeData);
    }

    internal RemoveTorrentRequest(string url, string token, string hash, bool removeData) : base(url, token)
    {
        SetParams(hash, removeData);
    }

    private void SetParams(string hash, bool removeData)
    {
        string action;
        if (removeData) action = "removedata";
        else            action = "remove";

        this.parameters = new ()
        {
            { "action", action },
            { "hash", hash }
        };
    }
}
