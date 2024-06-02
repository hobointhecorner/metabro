using System.Text.RegularExpressions;

namespace MB.Metadata.Text;

public static class Weight
{
    static List<string> Separators = new()
    {
        " ",
        "."
    };

    static string GetSeparatorRegex()
    {
        var separator = from sep in Separators
                        let escaped = System.Text.RegularExpressions.Regex.Escape(sep)
                        select escaped;

        return $"[{string.Join(null, separator)}]";
    }

    public static int GetFilenameWeight(TextMetadata metadata)
    {
        int result = 0;
        var ruleList = WeightRule.GetRules();

        bool hasTitle = metadata.Title != null;
        bool hasSeasonEpisode = metadata.HasSeasonEpisode;
        bool hasAirDate = metadata.HasAirdate;
        bool hasUploader = metadata.Uploader != null && metadata.Uploader != string.Empty;
        bool hasOtherInfo = metadata.OtherInfo != null;

        if (hasTitle) result += 50000;
        if (hasSeasonEpisode || hasAirDate) result += 50000;

        if (hasUploader)
        {
            var uploaderRules = from rule in ruleList
                                where rule.Type == WeightRuleType.Uploader
                                select rule;

            foreach (var rule in uploaderRules) result += rule.GetWeight(metadata.Uploader!);
        }

        if (hasOtherInfo)
        {
            var infoItems = System.Text.RegularExpressions.Regex.Split(metadata.OtherInfo!, GetSeparatorRegex());
            var otherRules = from rule in ruleList
                             where rule.Type != WeightRuleType.Uploader
                             select rule;

            foreach (string info in infoItems)
            {
                foreach (var rule in otherRules)
                {
                    result += rule.GetWeight(info);
                }
            }
        }

        return result;
    }
}
