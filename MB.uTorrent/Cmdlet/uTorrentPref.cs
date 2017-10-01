using MB.File;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MB.uTorrent
{
    public class TorrentPref
    {
        public string ComputerName { get; set; }
        public string Port { get; set; }
        public string Url { get { return string.Format("http://{0}:{1}/gui", ComputerName, Port); } }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> PrivateTrackers { get; set; }

        public TorrentPref()
        {
            PrivateTrackers = new List<string>();
        }

        public TorrentPref(string ComputerName, string Port, string Username, string Password)
        {
            this.ComputerName = ComputerName;
            this.Port = Port;
            this.Username = Username;
            this.Password = Password;

            PrivateTrackers = new List<string>();
        }

        public override string ToString()
        {
            return Url;
        }

        static string GetSubDirectory()
        {
            return "uTorrent";
        }

        static string GetFileName()
        {
            return "Settings.json";
        }

        public static TorrentPref GetPref()
        {
            return FileProvider.GetFile<TorrentPref>(GetFileName(), GetSubDirectory());
        }

        public void WritePref()
        {
            FileProvider.WriteFile<TorrentPref>(this, GetFileName(), GetSubDirectory());
        }
        [Cmdlet(VerbsCommon.Get, nameof(TorrentPref))]
        [OutputType(typeof(TorrentPref))]
        public class GetuTorrentPrefCmdlet : Cmdlet
        {
            protected override void BeginProcessing()
            {
                base.BeginProcessing();
            }

            protected override void ProcessRecord()
            {
                base.ProcessRecord();
                WriteObject(TorrentPref.GetPref());
            }
        }

        [Cmdlet(VerbsCommon.Set, nameof(TorrentPref))]
        public class SetuTorrentPrefCmdlet : Cmdlet
        {
            [Parameter(ValueFromPipeline = true)]
            public TorrentPref PrefObject { get; set; } = TorrentPref.GetPref();

            [Parameter]
            public string ComputerName { get; set; }

            [Parameter]
            public string Port { get; set; }

            [Parameter]
            public string Username { get; set; }

            [Parameter]
            public string Password { get; set; }

            protected override void ProcessRecord()
            {
                base.ProcessRecord();

                if (ComputerName != null) { PrefObject.ComputerName = ComputerName; }
                if (Port != null) { PrefObject.Port = Port; }
                if (Username != null) { PrefObject.Username = Username; }
                if (Password != null) { PrefObject.Password = Password; }

                PrefObject.WritePref();
            }
        }

        [Cmdlet(VerbsCommon.Add, "uTorrentPrivateTracker")]
        public class AdduTorrentPrivateTrackerCmdlet : Cmdlet
        {
            [Parameter(ValueFromPipeline = true)]
            public TorrentPref PrefObject { get; set; } = TorrentPref.GetPref();

            [Parameter(Position = 0)]
            public string Tracker { get; set; }

            protected override void ProcessRecord()
            {
                base.ProcessRecord();

                if (PrefObject.PrivateTrackers == null)
                {
                    PrefObject.PrivateTrackers = new List<string>() { Tracker };
                    PrefObject.WritePref();
                }
                else if (!PrefObject.PrivateTrackers.Contains(Tracker))
                {
                    PrefObject.PrivateTrackers.Add(Tracker);
                    PrefObject.WritePref();
                }
            }
        }

        [Cmdlet(VerbsCommon.Remove, "uTorrentPrivateTracker")]
        public class RemoveuTorrentPrivateTrackerCmdlet : Cmdlet
        {
            [Parameter(ValueFromPipeline = true)]
            public TorrentPref PrefObject { get; set; } = TorrentPref.GetPref();

            [Parameter (Position = 0)]
            public string Tracker { get; set; }

            protected override void ProcessRecord()
            {
                base.ProcessRecord();
                if (PrefObject.PrivateTrackers != null && PrefObject.PrivateTrackers.Contains(Tracker))
                {
                    PrefObject.PrivateTrackers.Remove(Tracker);
                    PrefObject.WritePref();
                }
            }
        }

        [Cmdlet(VerbsData.Initialize, nameof(TorrentPref))]
        public class InitializeuTorrentPrefCmdlet : PSCmdlet
        {
            protected override void ProcessRecord()
            {
                base.ProcessRecord();

                Host.UI.Write("Enter the computer name for your uTorrent server: ");
                string ComputerName = Host.UI.ReadLine();

                Host.UI.Write("Enter the port your uTorrent server uses to connect [8080]: ");
                string Port = Host.UI.ReadLine();

                if (Port == string.Empty)
                    Port = "8080";

                Host.UI.Write("Enter your username: ");
                string Username = Host.UI.ReadLine();

                Host.UI.Write("Enter your password: ");
                SecureString Password = Host.UI.ReadLineAsSecureString();

                //new TorrentPref(ComputerName, Port, Username, Password).WritePref();
            }
        }
    }
}
