using MB.uTorrent.Requests;
using System.Collections;
using System.Text.Json.Nodes;

namespace MB.uTorrent;

enum TorrentProperty
{
    Hash,
    Name,
    Status,
    Size,
    PercentDone,
    Downloaded,
    Uploaded,
    Remaining,
    Eta,
    Ratio,
    UploadSpeed,
    DownloadSpeed,
    Label,
    PeersConnected,
    PeersSwarm,
    SeedsConnected,
    SeedsSwarm,
    Availability,
    QueueOrder,
    StatusText,
    DateAdded,
    DateCompleted,
    Path
}

public class Torrent
{
    #region PROPS

    public string Hash { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? StatusText { get; set; }
    public int StatusValue { get; set; }
    public float Size { get; set; }
    public int PercentDone { get; set; }
    public float Downloaded { get; set; }
    public float Uploaded { get; set; }
    public float Remaining { get; set; }
    public TimeSpan Eta { get; set; }
    public float Ratio { get; set; }
    public string? Label { get; set; }
    public float PeersConnected { get; set; }
    public float PeersSwarm { get; set; }
    public float SeedsConnected { get; set; }
    public float SeedsSwarm { get; set; }
    public float Availability { get; set; }
    public int QueueOrder { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateCompleted { get; set; }
    public string? Path { get; set; }

    private readonly Dictionary<TorrentProperty, int> TorrentInfoIndex = new()
    {
        { TorrentProperty.Hash,           0 },
        { TorrentProperty.Name,           2 },
        { TorrentProperty.Status,         1 },
        { TorrentProperty.Size,           3 },
        { TorrentProperty.PercentDone,    4 },
        { TorrentProperty.Downloaded,     5 },
        { TorrentProperty.Uploaded,       6 },
        { TorrentProperty.Remaining,      18 },
        { TorrentProperty.Eta,            10 },
        { TorrentProperty.Ratio,          7 },
        { TorrentProperty.UploadSpeed,    8 },
        { TorrentProperty.DownloadSpeed,  9 },
        { TorrentProperty.Label,          11 },
        { TorrentProperty.PeersConnected, 12 },
        { TorrentProperty.PeersSwarm,     13 },
        { TorrentProperty.SeedsConnected, 14 },
        { TorrentProperty.SeedsSwarm,     15 },
        { TorrentProperty.Availability,   16 },
        { TorrentProperty.QueueOrder,     17 },
        { TorrentProperty.StatusText,     21 },
        { TorrentProperty.DateAdded,      23 },
        { TorrentProperty.DateCompleted,      24 },
        { TorrentProperty.Path,           26 }
    };

    #endregion

    #region Constructors

    public Torrent()
    {
        Hash = "test";
        Name = "test";
        Status = "test";
        StatusText = "test";
        StatusValue = 0;
        Size = 0;
        PercentDone = 0;
        Downloaded = 0;
        Uploaded = 0;
        Remaining = 0;
        Eta = TimeSpan.Zero;
        Ratio = 0;
        Label = "test";
        Path = "test";
        PeersConnected = 0;
        PeersSwarm = 0;
        SeedsConnected = 0;
        SeedsSwarm = 0;
        Availability = 0;
        QueueOrder = 0;
        DateAdded = DateTime.MinValue;
        DateCompleted = null;
    }

    public Torrent(string Hash) => this.Hash = Hash;

    public override string ToString() => Hash;

    /*
     * Data structure documentation: https://github.com/bittorrent/webui/wiki/Web-UI-API#torrentlabels-list
     */
    public Torrent(JsonNode torrentNode)
    {
        Hash = GetNodeValue<string>(TorrentProperty.Hash, torrentNode);
        Name = GetNodeValue<string>(TorrentProperty.Name, torrentNode);
        StatusText = GetNodeValue<string>(TorrentProperty.StatusText, torrentNode);
        Size = GetNodeValue<float>(TorrentProperty.Size, torrentNode);
        Downloaded = GetNodeValue<float>(TorrentProperty.Downloaded, torrentNode);
        Uploaded = GetNodeValue<float>(TorrentProperty.Uploaded, torrentNode);
        Remaining = GetNodeValue<float>(TorrentProperty.Remaining, torrentNode);
        Label = GetNodeValue<string>(TorrentProperty.Label, torrentNode);
        Path = GetNodeValue<string>(TorrentProperty.Path, torrentNode);
        PeersConnected = GetNodeValue<float>(TorrentProperty.PeersConnected, torrentNode);
        PeersSwarm = GetNodeValue<float>(TorrentProperty.PeersSwarm, torrentNode);
        SeedsConnected = GetNodeValue<float>(TorrentProperty.SeedsConnected, torrentNode);
        SeedsSwarm = GetNodeValue<float>(TorrentProperty.SeedsSwarm, torrentNode);
        QueueOrder = GetNodeValue<int>(TorrentProperty.QueueOrder, torrentNode);

        //
        // PercentDone
        //
        int percentDone = GetNodeValue<int>(TorrentProperty.PercentDone, torrentNode);
        PercentDone = GetPercentDone(percentDone);

        //
        // Status
        //
        int status = GetNodeValue<int>(TorrentProperty.Status, torrentNode);
        StatusValue = status;
        Status = GetStatus(status, PercentDone);

        //
        // Eta
        //
        float eta = GetNodeValue<float>(TorrentProperty.Remaining, torrentNode);
        Eta = GetEta(eta);

        //
        // Ratio
        //
        float ratio = GetNodeValue<float>(TorrentProperty.Ratio, torrentNode);
        Ratio = GetRatio(ratio);

        //
        // Availability
        //
        float availability = GetNodeValue<float>(TorrentProperty.Availability, torrentNode);
        Availability = GetAvailability(availability);

        //
        // Date added
        //
        long dateAdded = GetNodeValue<long>(TorrentProperty.DateAdded, torrentNode);
        DateAdded = GetDateFromEpoch(dateAdded);

        //
        // Date completed
        //
        long dateCompleted = GetNodeValue<long>(TorrentProperty.DateCompleted, torrentNode);
        if (dateCompleted > 0)
        {
            DateCompleted = GetDateFromEpoch(dateCompleted);
        }
        else
        {
            DateCompleted = null;
        }
    }

    #endregion

    #region Support

    T GetNodeValue<T>(TorrentProperty property, JsonNode torrentNode)
    {
        int index = TorrentInfoIndex[property];

        return torrentNode![index]!.GetValue<T>(); ;
    }

    static int GetPercentDone(int percentDone) => percentDone / 10;
    static float GetRatio(float ratio) => ratio / 1000;
    static float GetAvailability(float availability) => availability / 65536;

    static string GetStatus(int status, int percentDone)
    {
        BitArray b = new(new int[] { status });
        bool[] bits = new bool[b.Count];
        b.CopyTo(bits, 0);

        string statusString = string.Empty;

        if (bits[7])
        {
            if (percentDone < 100)
                statusString = "loaded";
            else
                statusString = "finished";
        }
        if (bits[6]) { statusString = "queued"; }
        if (bits[5]) { statusString = "paused"; }
        if (bits[4]) { statusString = "error"; }
        if (bits[3]) { statusString = "checked"; }
        if (bits[2]) { statusString = "start_after_check"; }
        if (bits[1]) { statusString = "checking"; }
        if (bits[0])
        {
            if (percentDone < 100)
                statusString = "started";
            else
                statusString = "seeding";
        }

        return statusString;
    }

    static TimeSpan GetEta(float eta)
    {
        const int secondsInMinute = 60;
        const int secondsInHour = secondsInMinute * 60;
        int hours = 0, minutes = 0, seconds = 0;
        decimal secondsRemaining = (decimal)eta;

        if (eta >= secondsInHour)
        {
            // Hours
            decimal totalHours = secondsRemaining / secondsInHour;
            hours = (int)Math.Floor(totalHours);
            secondsRemaining -= hours * secondsInHour;
        }

        if (eta >= secondsInMinute)
        {
            // Minutes
            decimal totalMinutes = secondsRemaining / secondsInMinute;
            minutes = (int)Math.Floor(totalMinutes);
            secondsRemaining -= minutes * secondsInMinute;
        }

        // Seconds
        seconds = (int)secondsRemaining;

        return new TimeSpan(hours, minutes, seconds);
    }

    static DateTime GetDateFromEpoch(long seconds)
    {
        DateTime start = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return start.AddSeconds(seconds).ToLocalTime();
    }

    #endregion

    #region Class methods

    //
    // GET
    //

    public static async Task<List<Torrent>> GetTorrentAsync(Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);
        GetTorrentRequest request = new(client);
        JsonNode response = await client.JsonRequestAsync(request);

        JsonNode torrentsNode = response!["torrents"]!;

        var torrents = from JsonNode torrent in (JsonArray)torrentsNode
                       let torrentObject = new Torrent(torrent!)
                       select torrentObject;

        return torrents.ToList();
    }

    public static async Task<Torrent?> GetTorrentAsync(string hash, Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);
        var torrentList = await GetTorrentAsync(client);

        var torrent = from torrentItem in torrentList
                      where torrentItem.Hash == hash
                      select torrentItem;

        if (torrent.Any()) return torrent.First();
        else               return null;
    }

    public static List<Torrent> GetTorrent(Client? client = null) => GetTorrentAsync(client).GetAwaiter().GetResult();
    public static Torrent? GetTorrent(string hash, Client? client = null) => GetTorrentAsync(hash, client).GetAwaiter().GetResult();

    //
    // ADD
    //

    public static async Task AddTorrentAsync(string link, Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);

        AddTorrentLinkRequest request = new(link, client);
        HttpResponseMessage response = await client.RequestAsync(request);
    }

    public static void AddTorrent(string link, Client? client = null) => AddTorrentAsync(link, client).GetAwaiter().GetResult();

    //
    // START
    //

    public async Task StartAsync(Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);

        StartTorrentRequest request = new(this.Hash, client);
        HttpResponseMessage response = await client.RequestAsync(request);
    }

    public void Start(Client? client = null) => StartAsync(client).GetAwaiter().GetResult();

    //
    // STOP
    //

    public async Task StopAsync(Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);

        StopTorrentRequest request = new(this.Hash, client);
        HttpResponseMessage response = await client.RequestAsync(request);
    }

    public void Stop(Client? client = null) => StopAsync(client).GetAwaiter().GetResult();

    //
    // PAUSE
    //

    public async Task PauseAsync(Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);

        PauseTorrentRequest request = new(this.Hash, client);
        HttpResponseMessage response = await client.RequestAsync(request);
    }

    public void Pause(Client? client = null) => PauseAsync(client).GetAwaiter().GetResult();

    //
    // REMOVE
    //

    public async Task RemoveAsync(bool removeData = false, Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);

        RemoveTorrentRequest request = new(this.Hash, removeData, client);
        HttpResponseMessage response = await client.RequestAsync(request);
    }

    public void Remove(bool removeData = false, Client? client = null) => RemoveAsync(removeData, client).GetAwaiter().GetResult();

    //
    // LABEL
    //

    public async Task SetLabelAsync(string label, Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);

        SetTorrentLabelRequest request = new(this.Hash, label, client);
        HttpResponseMessage response = await client.RequestAsync(request);
    }

    public void SetLabel(string label, Client? client = null) => SetLabelAsync(label, client).GetAwaiter().GetResult();

    #endregion
}

