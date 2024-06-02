using MB.uTorrent.Requests;
using System.Text.Json.Nodes;

namespace MB.uTorrent;

enum TorrentFileProperty
{
    Name,
    Size,
    Downloaded,
    Priority
}

public class TorrentFile
{
    #region PROPS

    public int? Index { get; set; } = null;
    public string Name { get; set; }
    public float Size { get; set; }
    public float Downloaded { get; set; }
    public string Priority { get; set; }
    public string? Hash { get; set; } = null;

    public float PercentDone
    {
        get
        {
            double percentDone = Downloaded / Size * 100;
            return (float)Math.Round(percentDone, 2);
        }
    }

    public string Status
    {
        get
        {
            if (PercentDone == 100)
            {
                return "finished";
            }
            else if (Priority == "skip")
            {
                return "skipped";
            }
            else
            {
                return "downloading";
            }
        }
    }

    private readonly Dictionary<int, string> TorrentFilePriority = new Dictionary<int, string>()
    {
        { 0, "skip" },
        { 1, "low" },
        { 2, "normal" },
        { 3, "high" }
    };

    private readonly Dictionary<TorrentFileProperty, int> TorrentFileInfoIndex = new Dictionary<TorrentFileProperty, int>()
    {
        { TorrentFileProperty.Name,       0 },
        { TorrentFileProperty.Size,       1 },
        { TorrentFileProperty.Downloaded, 2 },
        { TorrentFileProperty.Priority,   3 },
    };

    #endregion

    #region Constructors

    public TorrentFile()
    {
        Index = 0;
        Name = "test";
        Size = 0;
        Downloaded = 0;
        Priority = "test";
        Hash = "test";
    }

    /*
     * Data structure documentation: https://github.com/bittorrent/webui/wiki/Web-UI-API#files-list
     */
    public TorrentFile(JsonNode torrentFileNode, string? hash = null, int? index = null)
    {
        Name = GetNodeValue<string>(TorrentFileProperty.Name, torrentFileNode);
        Size = GetNodeValue<float>(TorrentFileProperty.Size, torrentFileNode);
        Downloaded = GetNodeValue<float>(TorrentFileProperty.Downloaded, torrentFileNode);

        int priority = GetNodeValue<int>(TorrentFileProperty.Priority, torrentFileNode);
        Priority = GetPriority(priority);

        if (hash != null)
            this.Hash = hash;

        if (index != null)
            this.Index = index;
    }

    #endregion

    #region Support

    T GetNodeValue<T>(TorrentFileProperty property, JsonNode torrentFileNode)
    {
        T output;

        int index = TorrentFileInfoIndex[property];
        output = torrentFileNode![index]!.GetValue<T>();

        return output;
    }

    string GetPriority(int priority) => TorrentFilePriority[priority];

    static List<TorrentFile> GetTorrentFilesFromNode(JsonNode response, string hash)
    {
        List<TorrentFile> output = new List<TorrentFile>();
        JsonNode filesNode = response!["files"]!;
        JsonArray fileHashList = (JsonArray)filesNode;
        int fileIndex = 0;

        for (int i = 0; i < fileHashList.Count; i++)
        {
            if (i % 2 != 0)
            {
                JsonArray fileList = (JsonArray)fileHashList[i]!;
                output.AddRange(
                    from JsonNode file in fileList
                    let torrentFileObject = new TorrentFile(file!, hash, fileIndex)
                    select torrentFileObject
                );

                fileIndex++;
            }
            else
            {
                // This is just the torrent hash.  I hate this API.
            }
        }

        return output;
    }

    #endregion

    #region Class methods

    public static List<TorrentFile> GetTorrentFile(string hash, Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);
        GetTorrentFileRequest request = new GetTorrentFileRequest(hash, client);
        JsonNode response = client.JsonRequest(request);

        return GetTorrentFilesFromNode(response, hash);
    }

    public static async Task<List<TorrentFile>> GetTorrentFileAsync(string hash, Client? client = null)
    {
        if (client == null) client = Client.GetClient(null);

        List<TorrentFile> output = new List<TorrentFile>();
        GetTorrentFileRequest request = new GetTorrentFileRequest(hash, client);
        JsonNode response = await client.JsonRequestAsync(request);

        return GetTorrentFilesFromNode(response, hash);
    }

    #endregion
}
