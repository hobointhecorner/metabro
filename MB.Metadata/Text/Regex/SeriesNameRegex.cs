using System.Text.RegularExpressions;

namespace MB.Metadata.Text.Regex;

internal class SeriesNameRegex : MetadataRegex
{
    internal static readonly List<char> restrictedCharacters = new() { '/', '[', ']', '*', '&', '%', '$', '#', '@', '-' };

    // items used here need escaped at some point
    internal static readonly List<OptionalCharacterRegex> optionalCharacters = OptionalCharacterRegex.GetOptionalCharacters();
    internal static readonly Dictionary<string, List<string>> multiMatchStrings = new()
    {
        { " ", new List<string> { " ", @"\." } }
    };

    internal static string GetRestrictedString(string input)
    {
        string result = input;

        // Remove restricted characters
        foreach (char character in restrictedCharacters)
        {
            if (result.Contains(character))
            {
                result = result.Replace(character.ToString(), string.Empty);
            }
        }

        return result;
    }

    internal static string GetOptionalString(string input)
    {
        string result = input;

        // Replace any present optional characters
        foreach (OptionalCharacterRegex character in optionalCharacters)
        {
            string compareString = character.Character.ToString();
            if (character.Escape) compareString = System.Text.RegularExpressions.Regex.Escape(compareString);

            if (result.Contains(compareString))
            {
                if (character.Escape) result = result.Replace(compareString, $"\\{character}?");
                else                  result = result.Replace(compareString, $"{character}?");
            }
        }

        return result;
    }
    internal static string GetMultimatchString(string input)
    {
        string result = input;

        // Replace regex-escaped characters with multiple potential matches
        foreach (var multiMatchString in multiMatchStrings)
        {
            string matchString = System.Text.RegularExpressions.Regex.Escape(multiMatchString.Key);
            var multiMatchStrings = multiMatchString.Value;

            if (result.Contains(matchString))
            {
                string charSet = $"[{string.Join("", multiMatchStrings)}]";
                result = result.Replace(matchString, charSet);
            }
        }

        return result;
    }

    internal static string GetSafeString(string input, bool escape = true)
    {
        string result = input.ToLower();

        result = GetRestrictedString(result);

        // Strip additional white space
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ").Trim();

        // Escape regex character for later processing
        if (escape) result = System.Text.RegularExpressions.Regex.Escape(result);

        return result;
    }

    internal static string GetRegexString(string input)
    {
        string result = input;

        result = GetSafeString(result);
        result = GetOptionalString(result);
        result = GetMultimatchString(result);

        return result;
    }

    public static string GetSeriesName(string input, string[]? seriesNames = null)
    {
        if (seriesNames != null)
        {
            foreach (string seriesName in seriesNames)
            {
                string seriesRegexString = GetRegexString(seriesName);

                if (System.Text.RegularExpressions.Regex.IsMatch(input, seriesRegexString, RegexOptions.IgnoreCase))
                {
                    return seriesName;
                }
            }
        }

        return input;
    }
}
