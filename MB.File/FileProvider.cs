using Newtonsoft.Json;

namespace MB.File;

public static class FileProvider
{
    static string GetBasePath(string? subDirectory = null)
    {
        string path = "metabro";
        if (subDirectory != null) path = Path.Combine(path, subDirectory);

        string userConfigDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); ;

        return Path.Combine(userConfigDir, path);
    }
    static string GetFileParentPath(string path)
    {
        return Directory.GetParent(path)!.FullName;
    }
    public static string GetFilePath(string FileName, string? Subdirectory = null)
    {
        return Path.Combine(GetBasePath(Subdirectory), FileName);
    }

    static bool TestDirectory(string path)
    {
        if (Directory.Exists(path))
            return true;
        else
            return false;
    }
    public static bool TestFile(string fileName, string? subDirectory = null)
    {
        string path = GetFilePath(fileName, subDirectory);
        if (System.IO.File.Exists(@path))
            return true;
        else
            return false;
    }

    static void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }
    static void CreateFile(string path)
    {
        try
        {
            string parentDirectory = GetFileParentPath(path);
            if (!TestDirectory(parentDirectory))
                CreateDirectory(parentDirectory);

            System.IO.File.Create(path).Close();
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create pref file, error message:" + e.Message);
        }
    }

    public static T GetFile<T>(string fileName, string? subDirectory = null) where T : new()
    {
        if (TestFile(fileName, subDirectory))
        {
            string path = GetFilePath(fileName, subDirectory);
            using StreamReader file = System.IO.File.OpenText(path);

            JsonSerializer serializer = new();
            T outputObject = (T)serializer.Deserialize(file, typeof(T))!;

            return outputObject!;
        }
        else
        {
            return default!;
        }
    }

    public static string? GetFile(string fileName, string? subDirectory = null)
    {
        if (TestFile(fileName, subDirectory!))
        {
            string path = GetFilePath(fileName, subDirectory);
            return System.IO.File.ReadAllText(path);
        }
        else
        {
            return null;
        }
    }

    public static void WriteFile<T>(T content, string fileName, string? subDirectory = null)
    {
        string path = GetFilePath(fileName, subDirectory);
        string serializedJson = JsonConvert.SerializeObject(content, Formatting.Indented);

        if (!TestFile(fileName, subDirectory))
            CreateFile(path);

        System.IO.File.WriteAllText(@path, serializedJson);
    }

    public static void WriteFile(string content, string fileName, string? subDirectory = null)
    {
        string path = GetFilePath(fileName, subDirectory);
        if (!TestFile(fileName, subDirectory))
            CreateFile(path);
        System.IO.File.WriteAllText(@path, content);
    }

    public static void RemoveFile(string fileName, string? subDirectory = null)
    {
        string path = GetFilePath(fileName, subDirectory);
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
    }
}
