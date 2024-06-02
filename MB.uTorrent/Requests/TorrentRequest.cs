namespace MB.uTorrent.Requests;

internal class TorrentRequest
{
    public string Url { get; set; } = string.Empty;
    protected internal string Token = string.Empty;

    internal Dictionary<string, string> parameters = new();

    public TorrentRequest() { }

    public TorrentRequest(string Url, string Token)
    {
        this.Url = Url;
        this.Token = Token;
    }

    public TorrentRequest(Client client)
    {
        this.Url = client.Url!;
        this.Token = client.Token!;
    }

    public HttpMethod GetMethod(HttpMethod? method)
    {
        if (method != null)
        {
            return method;
        }
        else
        {
            return HttpMethod.Get;
        }
    }

    public string FormatRequestParameters(Dictionary<string, string> parameters)
    {
        List<string> kvpList = new List<string>();

        if (Token != String.Empty)
        {
            kvpList.Add($"token={Token}");
        }

        foreach (string name in parameters.Keys)
        {
            if (parameters[name] != String.Empty)
            {
                kvpList.Add($"{name}={parameters[name]}");
            }
            else
            {
                kvpList.Add(name);
            }
        }

        return string.Join('&', kvpList);
    }

    public HttpRequestMessage GetRequest(HttpMethod? method = null) =>
        GetRequest(parameters, method);

    public HttpRequestMessage GetRequest(string resource, HttpMethod? method = null) =>
        new(GetMethod(method), $"{Url}/{resource}");

    public HttpRequestMessage GetRequest(Dictionary<string, string> parameters, HttpMethod? method = null) =>
        new(GetMethod(method), $"{Url}/?{FormatRequestParameters(parameters)}");

    public HttpRequestMessage GetRequest(string resource, Dictionary<string, string> parameters, HttpMethod? method = null) =>
        new(GetMethod(method), $"{Url}/{resource}?{FormatRequestParameters(parameters)}");
}
