using MB.Metadata.Text.Regex;
using System.Text.RegularExpressions;

namespace MB.Metadata.Text;

public class TextMetadata
{
    public string FileName;
    public string? Title { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
    public DateTime? AirDate { get; set; }
    public string? Uploader { get; set; } = null;
    public string? OtherInfo { get; set; }
    public int Weight { get; set; } = 0;

    public bool HasSeasonEpisode
    {
        get
        {
            return this.SeasonNumber != null && this.EpisodeNumber != null;
        }
    }
    public bool HasAirdate
    {
        get
        {
            return this.AirDate != null && this.AirDate.HasValue;
        }
    }

    internal static Dictionary<string, string> components => new()
    {
        { "title",     @"(?<title>.+?)"},
        { "otherInfo", @"(?<otherInfo>.+?)" },
        { "uploader",  @"(-(?<uploader>\w+))?" }
    };

    public TextMetadata(string filename)
    {
        this.FileName = filename;
    }

    public TextMetadata(string filename, string[]? seriesNames) : this(filename)
    {
        if (seriesNames != null) this.Parse(seriesNames);
    }

    internal static string GetCaptureGroupValue(string name, GroupCollection groups)
    {
        if (groups.ContainsKey(name)) return groups[name].Value;
        else                          return string.Empty;
    }

    public void Parse(string[]?seriesNames = null)
    {
        string airdateUngrouped = $"({string.Join(")|(", AirdateRegex.GetRegex(false))})";
        string seasonEpisodeUngrouped = $"({string.Join(")|(", SeasonEpisodeRegex.GetRegex(false))})";

        string shouldProcessRegex = $"^{components["title"]}.((?<airdate>{airdateUngrouped})|(?<seasonEpisode>{seasonEpisodeUngrouped})).{components["otherInfo"]}{components["uploader"]}$";
        Match shouldProcessMatch = System.Text.RegularExpressions.Regex.Match(this.FileName, shouldProcessRegex);

        if (shouldProcessMatch.Success)
        {
            bool hasSeasonEpisode = GetCaptureGroupValue("seasonEpisode", shouldProcessMatch.Groups) != string.Empty;
            bool hasAirdate = GetCaptureGroupValue("airdate", shouldProcessMatch.Groups) != string.Empty;

            string title = GetCaptureGroupValue("title", shouldProcessMatch.Groups);
            this.Title = SeriesNameRegex.GetSeriesName(title, seriesNames);

            this.Uploader = GetCaptureGroupValue("uploader", shouldProcessMatch.Groups);
            this.OtherInfo = GetCaptureGroupValue("otherInfo", shouldProcessMatch.Groups);


            if (hasSeasonEpisode)
            {
                foreach (string regexSegment in SeasonEpisodeRegex.GetRegex(true))
                {
                    string snEpRegex = $"^{components["title"]}.{regexSegment}.{components["otherInfo"]}{components["uploader"]}$";
                    Match snEpMatch = System.Text.RegularExpressions.Regex.Match(this.FileName, snEpRegex);

                    string season = GetCaptureGroupValue("season", snEpMatch.Groups);
                    string episode = GetCaptureGroupValue("episode", snEpMatch.Groups);

                    if (season != string.Empty && episode != string.Empty)
                    {
                        this.SeasonNumber = Convert.ToInt32(season);
                        this.EpisodeNumber = Convert.ToInt32(episode);
                        break;
                    }
                }
            }
            else if (hasAirdate)
            {
                foreach (string regexSegment in AirdateRegex.GetRegex(true))
                {
                    string airdateRegex = $"^{components["title"]}.{regexSegment}.{components["otherInfo"]}{components["uploader"]}$";
                    Match airdateMatch = System.Text.RegularExpressions.Regex.Match(this.FileName, airdateRegex);

                    string year = GetCaptureGroupValue("year", airdateMatch.Groups);
                    string month = GetCaptureGroupValue("month", airdateMatch.Groups);
                    string day = GetCaptureGroupValue("day", airdateMatch.Groups);

                    if (year != string.Empty && month != string.Empty && day != string.Empty)
                    {
                        this.AirDate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day), 0, 0, 0);
                        break;
                    }
                }
            }
        }

        this.Weight = Metadata.Text.Weight.GetFilenameWeight(this);
    }

    public static bool FindTextMetadata(TextMetadata metadata, string? title, int? seasonNumber, int? episodeNumber, DateTime? airDate)
    {
        if (
            (title == null || TextUtils.TestSimpleMatch(title, metadata.Title!, true))
            && (seasonNumber == null || metadata.SeasonNumber == seasonNumber)
            && (episodeNumber == null || metadata.EpisodeNumber == episodeNumber)
            && (airDate == null || metadata.AirDate == airDate)
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
