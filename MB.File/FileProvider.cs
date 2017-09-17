using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MB.File
{
    public static class FileProvider
    {
        static string GetBasePath(string Subdirectory = null)
        {
            string path;

            if (Subdirectory != null)
                path = string.Format("Metabro\\{0}", Subdirectory);
            else
                path = "Metabro";

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path);
        }
        static string GetFileParentPath (string Path)
        {
            return Directory.GetParent(@Path).FullName;
        }
        public static string GetFilePath(string FileName, string Subdirectory = null)
        {
            return Path.Combine(GetBasePath(Subdirectory), FileName);
        }

        static bool TestDirectory(string Path)
        {
            if (Directory.Exists(Path))
                return true;
            else
                return false;
        }
        static bool TestFile(string FileName, string SubDirectory = null)
        {
            string path = GetFilePath(FileName, SubDirectory);
            if (System.IO.File.Exists(@path))
                return true;
            else
                return false;
        }

        static void CreateDirectory(string Path)
        {
            Directory.CreateDirectory(@Path);
        }
        static void CreateFile(string Path)
        {
            try
            {
                string parentDirectory = GetFileParentPath(Path);
                if (!TestDirectory(parentDirectory))
                    CreateDirectory(parentDirectory);

                System.IO.File.Create(@Path).Close();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create pref file, error message:" + e.Message);
            }
        }        

        public static T GetFile<T>(string FileName, string Subdirectory = null) where T : new()
        {
            if (TestFile(FileName, Subdirectory))
            {
                string path = GetFilePath(FileName, Subdirectory);
                using (StreamReader file = System.IO.File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    T outputObject = (T)serializer.Deserialize(file, typeof(T));
                    return outputObject;
                }
            }
            else
            {
                return default(T);
            }
        }

        public static string GetFile(string FileName, string Subdirectory = null)
        {
            if (TestFile(FileName, Subdirectory))
            {
                string path = GetFilePath(FileName, Subdirectory);
                return System.IO.File.ReadAllText(path);
            }
            else
            {
                return null;
            }
        }

        public static void WriteFile<T>(T Content, string FileName, string Subdirectory = null)
        {
            string path = GetFilePath(FileName, Subdirectory);
            string serializedJson = JsonConvert.SerializeObject(Content, Formatting.Indented);

            if (!TestFile(FileName, Subdirectory))
                CreateFile(path);

            System.IO.File.WriteAllText(@path, serializedJson);
        }

        public static void WriteFile(string Content, string FileName, string Subdirectory = null)
        {
            string path = GetFilePath(FileName, Subdirectory);
            if (!TestFile(FileName, Subdirectory))
                CreateFile(path);
            System.IO.File.WriteAllText(@path, Content);
        }

        public static void RemoveFile(string FileName, string Subdirectory = null)
        {
            string path = GetFilePath(FileName, Subdirectory);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        public static void DownloadFile(string Uri, string FileName, string Subdirectory = null)
        {
            string path = GetFilePath(FileName, Subdirectory);

            if (!Directory.Exists(GetFileParentPath(path)))
                Directory.CreateDirectory(GetFileParentPath(path));

            new WebClient().DownloadFile(Uri, path);
        }
    }
}
