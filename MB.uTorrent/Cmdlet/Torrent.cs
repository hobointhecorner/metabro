using MB.File;
using MB.TextParsing;
using MB.TextParsing.QualityRules;
using MB.uTorrent.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MB.uTorrent
{
    public class Torrent
    {
        UTorrentAPI.Torrent ApiTorrent = null;
        public string Name { get; set; }
        public string Status { get; set; }
        public string[] StatusTag { get; set; }
        public string Label
        {
            get { return this.ApiTorrent.Label; }
            set { if (value != null) { this.ApiTorrent.Label = value; } }
        }
        public string Hash { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateCompleted { get; set; }
        public List<string> VideoFile { get; set; } = new List<string>();
        public List<string> File { get; set; } = new List<string>();
        public string Path { get; set; }

        public enum FileType
        {
            Torrent,
            Magnet
        }

        public enum RemovalOption
        {
            Torrent,
            Data,
            TorrentAndData
        }

        public Torrent()
        {

        }

        public Torrent(UTorrentAPI.Torrent ApiObject)
        {
            this.ApiTorrent = ApiObject;
            this.Name = ApiObject.Name;
            this.Status = ApiObject.StatusMessage;
            this.StatusTag = ApiObject.Status.ToString().Split(',');
            this.Label = ApiObject.Label;
            this.Hash = ApiObject.Hash;
            this.DateAdded = ApiObject.DateAdded;
            this.DateCompleted = ApiObject.DateCompleted;
            this.Path = ApiObject.SavePath;

            this.GetVideoFile(ApiObject);
            this.GetFile(ApiObject);
        }

        public Torrent(string Hash)
        {
            Torrent ApiObject = GetTorrent(Hash).FirstOrDefault();
            this.ApiTorrent = ApiObject.ApiTorrent;
            this.Name = ApiObject.Name;
            this.Status = ApiObject.Status;
            this.StatusTag = ApiObject.Status.ToString().Split(',');
            this.Label = ApiObject.Label;
            this.Hash = ApiObject.Hash;
            this.DateAdded = ApiObject.DateAdded;
            this.DateCompleted = ApiObject.DateCompleted;
            this.Path = ApiObject.Path;
            this.File = ApiObject.File;
            this.VideoFile = ApiObject.VideoFile;
        }

        public override string ToString()
        {
            return this.Name;
        }

        void GetFile(UTorrentAPI.Torrent ApiObject)
        {
            List<string> output = new List<string>();
            if (ApiObject.Files != null)
            {
                foreach (var file in ApiObject.Files)
                    output.Add(string.Format("{0}\\{1}", Path, file.Path));
            }

            File = output;
        }

        void GetVideoFile(UTorrentAPI.Torrent ApiObject)
        {
            List<string> output = new List<string>();
            if (ApiObject.Files != null)
            {
                List<QualityRule> qualityRules = FileQualityRule.GetQualityRule("Extension");

                foreach (var file in ApiObject.Files)
                    foreach (var rule in qualityRules)
                        if (QualityRule.TestQualityRule(rule, file.Path) > 0)
                        {
                            output.Add(string.Format("{0}\\{1}", Path, file.Path));
                            break;
                        }
            }

            VideoFile = output;
        }
                

        public static List<Torrent> GetTorrent(uTorrentClient ApiClient = null)
        {
            if (ApiClient == null)
                ApiClient = new uTorrentClient();

            List<Torrent> output = new List<Torrent>();
            var torrentList = ApiClient.GetTorrent();

            if (torrentList != null)
                foreach (UTorrentAPI.Torrent torrent in torrentList)
                    output.Add(new Torrent(torrent));

            return output;
        }

        public static List<Torrent> GetTorrent(string Hash, uTorrentClient ApiClient = null)
        {
            List<Torrent> torrentList = GetTorrent(ApiClient);
            List<Torrent> output = new List<Torrent>();

            if (torrentList != null)
            {
                var outputList = from Torrent t in torrentList
                                 where t.Hash == Hash
                                 select t;

                if (outputList != null)
                    output.Add(outputList.FirstOrDefault());
            }

            return output;
        }

        public static void AddTorrent(string Path, string SavePath = null, uTorrentClient ApiClient = null)
        {
            if (ApiClient == null)
                ApiClient = new uTorrentClient();

            if (SavePath == null)
                SavePath = StorageDirectory.GetStorageDirectory()[0].Name;

            bool deleteFile = false;

            FileType fileType;
            if (Path.ToLower().EndsWith(".torrent"))
            {
                fileType = FileType.Torrent;
            }
            else if (Path.ToLower().StartsWith("http"))
            {
                fileType = FileType.Torrent;
                deleteFile = true;
                string fileName = DateTime.Now.ToFileTime() + ".torrent";
                string downloadPath = Path;
                string subDirectory = @"uTorrent\Temp";
                Path = FileProvider.GetFilePath(fileName, subDirectory);
                FileProvider.DownloadFile(downloadPath, fileName, subDirectory);
            }
            else
            {
                fileType = FileType.Magnet;
            }

            ApiClient.AddTorrent(Path, fileType, SavePath, deleteFile);
        }

        public void RemoveTorrent(uTorrentClient ApiClient = null, RemovalOption RemovalOption = RemovalOption.Torrent)
        {
            if (ApiClient == null)
                ApiClient = new uTorrentClient();

            switch (RemovalOption)
            {
                case RemovalOption.Torrent:
                    ApiClient.RemoveTorrent(ApiTorrent, UTorrentAPI.TorrentRemovalOptions.Job);
                    break;

                case RemovalOption.Data:
                    ApiClient.RemoveTorrent(ApiTorrent, UTorrentAPI.TorrentRemovalOptions.Data);
                    break;

                case RemovalOption.TorrentAndData:
                    ApiClient.RemoveTorrent(ApiTorrent, UTorrentAPI.TorrentRemovalOptions.TorrentFileAndData);
                    break;
            }
        }

        public void SetLabel(string Label)
        {
            if (this.ApiTorrent != null)
            {
                this.ApiTorrent.Label = Label;
            }
        }

        public void StartTorrent(bool Force = false)
        {
            ApiTorrent.Start(Force);
        }

        public void StopTorrent()
        {
            ApiTorrent.Stop();
        }

        public void PauseTorrent()
        {
            ApiTorrent.Pause();
        }
    }

    public class StorageDirectory
    {
        public string Name { get; set; }

        public StorageDirectory()
        {

        }

        public StorageDirectory(string Path)
        {
            this.Name = Path;
        }

        public StorageDirectory(UTorrentAPI.Directory Directory)
        {
            this.Name = Directory.Path;
        }

        public override string ToString()
        {
            return Name;
        }

        public static List<StorageDirectory> GetStorageDirectory(uTorrentClient ApiClient = null)
        {
            if (ApiClient == null)
                ApiClient = new uTorrentClient();

            List<StorageDirectory> output = new List<StorageDirectory>();
            var directoryList = ApiClient.GetStorageDirectory();

            if (directoryList != null)
                foreach (UTorrentAPI.Directory directory in directoryList)
                    output.Add(new StorageDirectory(directory));

            return output;
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(Torrent))]
    [OutputType(typeof(List<Torrent>))]
    public class GetTorrentCmdlet : PSCmdlet
    {
        [Parameter]
        public string Name { get; set; } = "*";

        [Parameter]
        public string Hash { get; set; }

        [Parameter]
        public bool Dispose { get; set; } = true;

        uTorrentClient ApiClient = new uTorrentClient();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            List<Torrent> torrentList = new List<Torrent>();
            if (Hash == null)
                torrentList = Torrent.GetTorrent(ApiClient);
            else
                torrentList = Torrent.GetTorrent(Hash, ApiClient);

            if (torrentList != null)
                foreach (Torrent torrent in torrentList)
                    if (TextParser.TestSimpleMatch(Name, torrent.Name))
                        WriteObject(torrent);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            if (Dispose)
                ApiClient.Dispose();
        }
    }

    [Cmdlet(VerbsCommon.Add, nameof(Torrent))]
    public class AddTorrentCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public List<string> Path { get; set; }

        [Parameter]
        public StorageDirectory StorageDirectory = StorageDirectory.GetStorageDirectory()[0];

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        uTorrentClient ApiClient = new uTorrentClient();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            foreach (string p in Path)
            {
                Torrent.AddTorrent(p, StorageDirectory.Name, ApiClient);
                if (PassThru)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                    var torrentList = from Torrent t in Torrent.GetTorrent()
                                      orderby t.DateAdded descending
                                      select t;

                    if (torrentList != null)
                        WriteObject(torrentList.FirstOrDefault());
                }
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            ApiClient.Dispose();
        }
    }

    [Cmdlet(VerbsCommon.Remove, nameof(Torrent))]
    public class RemoveTorrentCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public List<Torrent> Torrent { get; set; }

        [Parameter]
        public Torrent.RemovalOption RemovalOption { get; set; } = uTorrent.Torrent.RemovalOption.Torrent;

        uTorrentClient ApiClient = new uTorrentClient();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            foreach (Torrent t in Torrent)
                t.RemoveTorrent(ApiClient, RemovalOption);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            ApiClient.Dispose();
        }
    }

    [Cmdlet(VerbsLifecycle.Start, nameof(Torrent))]
    public class StartTorrentCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public List<Torrent> Torrent { get; set; }

        [Parameter]
        public SwitchParameter Force { get; set; }

        uTorrentClient ApiClient = new uTorrentClient();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            foreach (Torrent t in Torrent)
                t.StartTorrent(Force);
            
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            ApiClient.Dispose();
        }
    }

    [Cmdlet(VerbsLifecycle.Stop, nameof(Torrent))]
    public class StopTorrentCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public List<Torrent> Torrent { get; set; }

        uTorrentClient ApiClient = new uTorrentClient();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            foreach (Torrent t in Torrent)
                t.StopTorrent();
                
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            ApiClient.Dispose();
        }
    }

    [Cmdlet(VerbsLifecycle.Suspend, nameof(Torrent))]
    public class PauseTorrentCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public List<Torrent> Torrent { get; set; }

        uTorrentClient ApiClient = new uTorrentClient();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            foreach (Torrent t in Torrent)
                t.PauseTorrent();
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            ApiClient.Dispose();
        }
    }

    [Cmdlet(VerbsCommon.Set, nameof(Torrent))]
    public class SetTorrentCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public List<Torrent> Torrent { get; set; }

        [Parameter (Position = 0)]
        public string Label { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            foreach (Torrent t in Torrent)
            {
                if (Label != null)
                    t.SetLabel(Label);
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(StorageDirectory))]
    [OutputType(typeof(List<StorageDirectory>))]
    public class GetTorrentStorageDirectoryCmdlet : PSCmdlet
    {
        [Parameter (Position = 0)]
        [ValidateNotNullOrEmpty()]
        public string Name { get; set; } = "*";

        uTorrentClient ApiClient = new uTorrentClient();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var directoryList = StorageDirectory.GetStorageDirectory(ApiClient);

            if (directoryList != null)
                foreach (StorageDirectory directory in directoryList)
                    if (TextParser.TestSimpleMatch(Name, directory.Name))
                        WriteObject(directory);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            ApiClient.Dispose();
        }
    }
}
