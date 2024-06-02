namespace MB.Metadata.Text.Regex;

public class MetadataRegex
{
    internal static string GetNamedGroup(string name, Dictionary<string, string> components)
    {
        return string.Format(@"(?<{0}>{1})", name, components[name]);
    }
}
