using MB.File;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby
{
    public class EmbyPref
    {
        public string ComputerName { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public bool? UseSSL { get; set; } = false;
        public string Url
        {
            get
            {
                string protocol;

                if ((bool)UseSSL)
                    protocol = "https";
                else
                    protocol = "http";

                return string.Format("{0}://{1}:{2}/emby", protocol, ComputerName, Port);
            }
        }
        public string ApiKey { get; set; } = string.Empty;
        public string DefaultUserId { get; set; } = string.Empty;
        public List<string>AutoPlaylistCollection { get; set; }
        public List<string> AutoPlaylistLibrary { get; set; }
        public List<String>AutoDownloadLibrary { get; set; }

        public EmbyPref()
        {
            AutoPlaylistCollection = new List<string>();
            AutoPlaylistLibrary = new List<string>();
            AutoDownloadLibrary = new List<string>();
        }

        public EmbyPref(string ComputerName, string Port, string ApiKey)
        {
            this.ComputerName = ComputerName;
            this.Port = Port;
            this.ApiKey = ApiKey;

            AutoPlaylistCollection = new List<string>();
            AutoPlaylistLibrary = new List<string>();
            AutoDownloadLibrary = new List<string>();
        }

        public static string GetSubDirectory()
        {
            return "Emby";
        }

        public static string GetFileName()
        {
            return "Settings.json";
        }

        public static EmbyPref GetPref()
        {
            return FileProvider.GetFile<EmbyPref>(GetFileName(), GetSubDirectory());
        }

        public void WritePref()
        {
            FileProvider.WriteFile<EmbyPref>(this, GetFileName(), GetSubDirectory());
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyPref))]
    [OutputType(typeof(EmbyPref))]
    public class GetEmbyPrefCmdlet : Cmdlet
    {
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            WriteObject(EmbyPref.GetPref());
        }
    }

    [Cmdlet(VerbsCommon.Set, nameof(EmbyPref))]
    public class SetEmbyPrefCmdlet : Cmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public EmbyPref PrefObject { get; set; } = EmbyPref.GetPref();

        [Parameter]
        public string ComputerName { get; set; }

        [Parameter]
        public string Port { get; set; }

        [Parameter]
        public bool? UseSSL { get; set; }

        [Parameter]
        public string ApiKey { get; set; }

        [Parameter]
        public string DefaultUserId { get; set; }


        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (ComputerName != null) { PrefObject.ComputerName = ComputerName; }
            if (Port != null) { PrefObject.Port = Port; }
            if (UseSSL != null) { PrefObject.UseSSL = UseSSL; }
            if (ApiKey != null) { PrefObject.ApiKey = ApiKey; }
            if (DefaultUserId != null) { PrefObject.DefaultUserId = DefaultUserId; }

            PrefObject.WritePref();
        }
    }
}
