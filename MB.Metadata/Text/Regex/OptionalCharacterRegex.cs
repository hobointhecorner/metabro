namespace MB.Metadata.Text.Regex;

internal class OptionalCharacterRegex
{
    public char Character { get; set; }
    public bool Escape { get; set; } = false;

    private static readonly List<char> unescaped = new() { '!', '\'' };
    private static readonly List<char> escaped = new() { '.', '?', '(', ')' };

    public OptionalCharacterRegex(char character, bool escape = false)
    {
        Character = character;
        Escape = escape;
    }

    public override string ToString()
    {
        return Character.ToString();
    }

    public static List<OptionalCharacterRegex> GetOptionalCharacters()
    {
        List<OptionalCharacterRegex> result = new();

        foreach (char c in unescaped) result.Add(new OptionalCharacterRegex(c, false));
        foreach (char c in escaped) result.Add(new OptionalCharacterRegex(c, true));

        return result;
    }
}
