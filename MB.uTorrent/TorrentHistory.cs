using MB.File;
using MB.Metadata.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.uTorrent;

public class TorrentHistory
{
    public string Title { get; set; }
    public string Hash { get; set; } = string.Empty;
    public string? Provider { get; set; } = null;
    public string? SeriesName { get; set; } = null;
    public int? SeasonNumber { get; set; } = null;
    public int? EpisodeNumber { get; set; } = null;
    public DateTime? AirDate { get; set; } = null;

    static string subDirectory = "utorrent";
    static string fileName = "history.json";

    public TorrentHistory(string title, string hash)
    {
        this.Title = title;
        this.Hash = hash;
    }

    TorrentHistory(TextMetadata metadata, string hash)
    {
        this.Title = metadata.FileName;
        this.Hash = hash;
        this.SeriesName = metadata.Title;
        this.SeasonNumber = metadata.SeasonNumber;
        this.EpisodeNumber = metadata.EpisodeNumber;
        this.AirDate = metadata.AirDate;
    }

    public TorrentHistory(TorrentInfo torrentInfo) : this((TextMetadata)torrentInfo, torrentInfo.Hash)
    {
        this.Provider = torrentInfo.Provider;
    }

    public TorrentHistory(Torrent torrent) : this(new TorrentInfo(torrent)) { }

    public static List<TorrentHistory>? GetTorrentHistory()
    {
        return FileProvider.GetFile<List<TorrentHistory>>(fileName, subDirectory);
    }

    public static void WriteTorrentHistory(List<TorrentHistory> history)
    {
        FileProvider.WriteFile(history, fileName, subDirectory);
    }

    public static void AddTorrentHistory(TorrentHistory newItem, int? maxItems = null)
    {
        var history = GetTorrentHistory();
        if (history == null) history = new();

        history.Add(newItem);

        if (maxItems != null)
        {
            int skipCount = history.Count - (int)maxItems;
            if (skipCount > 0)
            {
                history = history.
                            Skip(skipCount).
                            Take((int)maxItems).
                            ToList();
            }
        }

        WriteTorrentHistory(history);
    }

    public static void RemoveTorrentHistory(TorrentHistory removeItem)
    {
        var history = GetTorrentHistory();
        if (history == null) history = new();

        var filteredList = from item in history
                           where item.Hash != removeItem.Hash
                           select item;

        WriteTorrentHistory(filteredList.ToList());
    }
}
