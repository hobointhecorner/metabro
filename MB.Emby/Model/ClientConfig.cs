using EmbyClient.Dotnet.Client;
using MB.File;

namespace MB.Emby.Model;

public class ClientConfig
{
    public string? Url { get; set; } = null;
    public string? ApiKey { get; set; } = null;
    public string? DefaultUserId { get; set; } = null;

    internal static string subDirectory = "emby";
    internal static string fileName = "settings.json";

    public ClientConfig() { }

    public ClientConfig(string url, string apiKey)
    {
        Url = url;
        ApiKey = apiKey;
    }

    public override string? ToString()
    {
        return Url;
    }

    public static ClientConfig? GetConfig()
    {
        return FileProvider.GetFile<ClientConfig>(fileName, subDirectory);
    }

    public void WriteConfig()
    {
        FileProvider.WriteFile(this, fileName, subDirectory);
    }

    public Configuration GetApiConfig()
    {
        var config = new Configuration();
        config.BasePath = Url;
        config.DefaultHeader.Add("X-Emby-Token", ApiKey);

        return config;
    }
}
