using MB.File;
using MB.TextParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MB.RSS
{
    public enum TorrentProviderType
    {
        Feed,
        Search
    }

    public class RssTorrentProvider
    {
        public string Name { get; set; }
        public int Priority { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string SpaceDelimiter { get; set; }
        public string ProviderMap { get; set; } = "Default";

        public static string GetSubDirectory()
        {
            return "RSS";
        }

        public static string GetFileName()
        {
            return "Providers.json";
        }

        public static List<RssTorrentProvider> GetProvider()
        {
            return FileProvider.GetFile<List<RssTorrentProvider>>(GetFileName(), GetSubDirectory());
        }

        public static List<RssTorrentProvider> GetProvider(string Name, string Type = "*")
        {
            return (from RssTorrentProvider provider in GetProvider()
                    where TextParser.TestSimpleMatch(Name, provider.Name)
                    where TextParser.TestSimpleMatch(Type, provider.Type)
                    select provider).ToList();
        }

        public static void WriteProvider(List<RssTorrentProvider> TorrentProvider)
        {
            FileProvider.WriteFile<List<RssTorrentProvider>>(TorrentProvider, GetFileName(), GetSubDirectory());
        }
    }

    public class RssTorrentProviderAttributeMap
    {
        public string MapName { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string DatePublished { get; set; }

        public static string GetSubDirectory()
        {
            return "RSS";
        }

        public static string GetFileName()
        {
            return "ProviderMaps.json";
        }

        public static List<RssTorrentProviderAttributeMap> GetAttributeMap()
        {
            return FileProvider.GetFile<List<RssTorrentProviderAttributeMap>>(GetFileName(), GetSubDirectory());
        }

        public static void WriteAttibuteMap(List<RssTorrentProviderAttributeMap> AttributeMap)
        {
            FileProvider.WriteFile<List<RssTorrentProviderAttributeMap>>(AttributeMap, GetFileName(), GetSubDirectory());
        }

        public static RssTorrentProviderAttributeMap GetAttributeMap(string MapName)
        {
            return (from RssTorrentProviderAttributeMap map in GetAttributeMap()
                    where TextParser.TestSimpleMatch(MapName, map.MapName)
                    select map).FirstOrDefault();
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(RssTorrentProvider))]
    [OutputType(typeof(List<RssTorrentProvider>))]
    public class GetRssTorrentProviderCmdlet : PSCmdlet
    {
        [Parameter]
        public string Type { get; set; } = "*";

        [Parameter (Position = 0)]
        public string Name { get; set; } = "*";

        protected override void ProcessRecord()
        {
            List<RssTorrentProvider> output = RssTorrentProvider.GetProvider(Name, Type);

            if(output != null)
                foreach (RssTorrentProvider provider in output)
                    WriteObject(provider);
        }
    }
}
