using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using MB.uTorrent.Requests;

namespace MB.uTorrent;

public class Client : ClientConfig
{
    internal string? Token = null;

    private readonly AuthenticationHeaderValue? authHeader = null;
    private readonly HttpClient client = new();

    public Client(string url, string username, string password) : base(url, username, password)
    {
        this.authHeader = GetAuthHeader();
        //Connect();
    }

    public static Client GetClient(ClientConfig? client = null)
    {
        if (client == null)
        {
            ClientConfig? defaultConfig = ClientConfig.GetConfig();
            bool hasDefaultUrl = defaultConfig != null && defaultConfig.Url != null;
            bool hasDefaultCreds = defaultConfig != null && defaultConfig.Username != null && defaultConfig.Password != null;

            if (!hasDefaultUrl) throw new Exception("Client URL not set.  Use 'Set-TorrentClientConfig -Url <string>' to configure.");
            if (!hasDefaultCreds) throw new Exception("Client credentials not set.  Use 'Set-TorrentClientConfig -Credentials <PSCredential>' to configure.");

            return new Client(defaultConfig!.Url!, defaultConfig!.Username!, defaultConfig!.Password!);
        }
        else
        {
            return new Client(client!.Url!, client!.Username!, client!.Password!);
        }
    }

    AuthenticationHeaderValue GetAuthHeader()
    {
        string authString = $"{Username}:{Password}";
        string encodedAuthString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authString));
        return new AuthenticationHeaderValue("Basic", encodedAuthString);
    }

    internal HttpResponseMessage Request(HttpRequestMessage request)
    {
        request.Headers.Authorization = authHeader;

        HttpResponseMessage response = client.Send(request);
        response.EnsureSuccessStatusCode();

        return response;
    }

    internal HttpResponseMessage Request(TorrentRequest request) => Request(request.GetRequest());

    internal async Task<HttpResponseMessage> RequestAsync(HttpRequestMessage request)
    {

        request.Headers.Authorization = authHeader;

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return response;
    }

    internal async Task<HttpResponseMessage> RequestAsync(TorrentRequest request) => await RequestAsync(request.GetRequest());

    internal JsonNode JsonRequest(HttpRequestMessage request)
    {
        HttpResponseMessage response = this.Request(request);
        var responseStream = response.Content.ReadAsStream();
        var responseString = new StreamReader(responseStream).ReadToEnd();

        return JsonNode.Parse(responseString)!;
    }

    internal JsonNode JsonRequest(TorrentRequest request) => JsonRequest(request.GetRequest());

    internal async Task<JsonNode> JsonRequestAsync(HttpRequestMessage request)
    {
        HttpResponseMessage response = await this.RequestAsync(request);
        string responseString = await response.Content.ReadAsStringAsync();

        return JsonNode.Parse(responseString)!;
    }

    internal async Task<JsonNode> JsonRequestAsync(TorrentRequest request) => await JsonRequestAsync(request.GetRequest());
}