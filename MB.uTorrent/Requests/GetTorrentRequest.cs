namespace MB.uTorrent.Requests;

internal class GetTorrentRequest : TorrentRequest
{
    internal GetTorrentRequest(Client client) : base(client)
    {
        SetParams();
    }

    internal GetTorrentRequest(string Url, string Token) : base(Url, Token)
    {
        SetParams();
    }

    private void SetParams()
    {
        this.parameters = new Dictionary<string, string>()
        {
            { "list", "1" }
        };
    }
}
