using System.Text.RegularExpressions;

namespace MB.Metadata.Text;

public static class TextUtils
{
    public static int MinimumValidWeight = 100000;

    public static bool TestSimpleMatch(string input, string pattern, bool caseInsensitive = false)
    {
        string regString = System.Text.RegularExpressions.Regex.Escape(pattern.ToLower()).Replace(@"\*", ".*");

        if (caseInsensitive)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, $"^{regString}$", RegexOptions.IgnoreCase);
        }
        else
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, $"^{regString}$");
        }
    }

    public static string GetFileExtension(string filename)
    {
        int f = filename.LastIndexOf('.');
        int l = filename.Length - f;

        return filename.Substring(f, l);
    }
}
