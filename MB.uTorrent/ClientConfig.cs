using MB.File;

namespace MB.uTorrent;

public class ClientConfig
{
    public string? Url { get; set; } = null;
    public string? Username { get; set; } = null;
    public string? Password { get; set; } = null;
    public List<string> PrivateTrackers { get; set; } = new List<string>();
    public int MaxHistoryItems { get; set; } = 100;

    internal static string subDirectory = "utorrent";
    internal static string fileName = "settings.json";

    public ClientConfig() { }

    public ClientConfig(string Url, string Username, string Password)
    {
        this.Url = Url;
        this.Username = Username;
        this.Password = Password;
    }

    public override string ToString()
    {
        return Url!;
    }

    public static ClientConfig? GetConfig()
    {
        return FileProvider.GetFile<ClientConfig>(fileName, subDirectory);
    }

    public void WriteConfig()
    {
        FileProvider.WriteFile(this, fileName, subDirectory);
    }
}
