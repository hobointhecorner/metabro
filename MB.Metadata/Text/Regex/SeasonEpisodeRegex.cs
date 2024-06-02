namespace MB.Metadata.Text.Regex;

internal class SeasonEpisodeRegex : MetadataRegex
{
    internal static Dictionary<string, string> components => new()
    {
        { "season",  @"\d{1,4}" },
        { "episode", @"\d{1,3}" }
    };

    public static List<string> GetRegex(bool group = false)
    {
        string season, episode;

        if (group)
        {
            season = GetNamedGroup("season", components);
            episode = GetNamedGroup("episode", components);
        }
        else
        {
            season = components["season"];
            episode = components["episode"];
        }

        return new List<string>()
        {
          $"[Ss]{season}[Ee]{episode}",
          $"{season}[Xx]{episode}"
        };
    }
}
