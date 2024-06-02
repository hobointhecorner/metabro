namespace MB.Metadata.Text.Regex;

internal class AirdateRegex : MetadataRegex
{
    internal static Dictionary<string, string> components => new()
    {
        { "year",      @"\d{4}" },
        { "month",     @"\d{2}" },
        { "day",       @"\d{2}" },
        { "separator", @"[-/\. ]" }
    };

    public static List<string> GetRegex(bool group = false)
    {
        string year, month, day;
        string sep = components["separator"];

        if (group)
        {
            year = GetNamedGroup("year", components);
            month = GetNamedGroup("month", components);
            day = GetNamedGroup("day", components);
        }
        else
        {
            year = components["year"];
            month = components["month"];
            day = components["day"];
        }

        return new List<string>()
        {
          $"{year}{sep}{month}{sep}{day}",
          $"{month}{sep}{day}{sep}{year}",
          $"{year}{month}{day}"
        };
    }
}
