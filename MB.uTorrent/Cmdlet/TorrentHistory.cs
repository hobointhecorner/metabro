
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.File;
using System.Management.Automation;
using MB.uTorrent.Api;
using MB.TextParsing;

namespace MB.uTorrent
{
    public class TorrentHistory
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public string SeriesName { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
        public DateTime? AirDate { get; set; }
        public DateTime? Date { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        public TorrentHistory()
        {
            this.Guid = Guid.NewGuid();
        }

        public TorrentHistory(string Name, string Provider, string SeriesName, int? SeasonNumber, int? EpisodeNubmer, DateTime? AirDate, DateTime? Date)
        {
            this.Name = Name;
            this.Provider = Provider;
            this.SeriesName = SeriesName;
            this.SeasonNumber = SeasonNumber;
            this.EpisodeNumber = EpisodeNumber;
            this.AirDate = AirDate;
            this.Date = Date;
            this.Guid = Guid.NewGuid();
        }

        public override string ToString()
        {
            return Name;
        }

        static string GetSubDirectory()
        {
            return "uTorrent";
        }

        static string GetFileName()
        {
            return "TorrentHistory.json";
        }

        public static List<TorrentHistory> GetTorrentHistory()
        {
            return FileProvider.GetFile<List<TorrentHistory>>(GetFileName(), GetSubDirectory());
        }

        public static void WriteTorrentHistory(List<TorrentHistory> TorrentHistory)
        {
            FileProvider.WriteFile<List<TorrentHistory>>(TorrentHistory, GetFileName(), GetSubDirectory());
        }

        public static void AddTorrentHistory(TorrentHistory TorrentHistory)
        {
            List<TorrentHistory> historyList = GetTorrentHistory();
            if (historyList != null)
                historyList.Add(TorrentHistory);
            else
                historyList = new List<TorrentHistory>() { TorrentHistory };

            WriteTorrentHistory(historyList);
            
        }

        public static void RemoveTorrentHistory(TorrentHistory TorrentHistory)
        {
            List<TorrentHistory> historyList = GetTorrentHistory();

            if (historyList != null)
            {
                var historySelectedList = from TorrentHistory history in historyList
                                          where history.Guid != TorrentHistory.Guid
                                          select history;

                WriteTorrentHistory(historySelectedList.ToList<TorrentHistory>());
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(TorrentHistory))]
    [OutputType(typeof(List<TorrentHistory>))]
    public class GetTorrentHistoryCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var torrentHistoryList = TorrentHistory.GetTorrentHistory();

            if (torrentHistoryList != null)
                foreach (TorrentHistory TorrentHistory in torrentHistoryList)
                    WriteObject(TorrentHistory);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }

    [Cmdlet(VerbsCommon.Find, nameof(TorrentHistory))]
    [OutputType(typeof(List<TorrentHistory>))]
    public class FindTorrentHistoryCmdlet : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public List<TorrentHistory> TorrentHistory { get; set; } = MB.uTorrent.TorrentHistory.GetTorrentHistory();

        [Parameter]
        public string Name { get; set; } 

        [Parameter]
        public string Provider { get; set; }

        [Parameter]
        public string SeriesName { get; set; }

        [Parameter]
        public int? SeasonNumber { get; set; }

        [Parameter]
        public int? EpisodeNumber { get; set; }

        [Parameter]
        public DateTime? AirDate { get; set; }

        List<TorrentHistory> outputList;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            outputList = TorrentHistory;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Name != null && outputList != null)
            {
                var selectList = from TorrentHistory history in TorrentHistory
                                 where TextParser.TestSimpleMatch(Name, history.Name)
                                 select history;

                outputList = selectList.ToList<TorrentHistory>();
            }

            if (Provider != null && outputList != null)
            {
                var selectList = from TorrentHistory history in TorrentHistory
                                 where TextParser.TestSimpleMatch(Provider, history.Provider)
                                 select history;

                outputList = selectList.ToList<TorrentHistory>();
            }

            if (SeriesName != null && outputList != null)
            {
                var selectList = from TorrentHistory history in TorrentHistory
                                 where TextParser.TestSimpleMatch(SeriesName, history.SeriesName)
                                 select history;

                outputList = selectList.ToList<TorrentHistory>();
            }

            if (SeasonNumber != null && outputList != null)
            {
                var selectList = from TorrentHistory history in TorrentHistory
                                 where history.SeasonNumber == SeasonNumber
                                 select history;

                outputList = selectList.ToList<TorrentHistory>();
            }

            if (EpisodeNumber != null && outputList != null)
            {
                var selectList = from TorrentHistory history in TorrentHistory
                                 where history.EpisodeNumber == EpisodeNumber
                                 select history;

                outputList = selectList.ToList<TorrentHistory>();
            }

            if (AirDate != null && outputList != null)
            {
                var selectList = from TorrentHistory history in TorrentHistory
                                 where history.AirDate == AirDate
                                 select history;

                outputList = selectList.ToList<TorrentHistory>();
            }

            if (outputList != null)
                foreach (TorrentHistory history in outputList)
                    WriteObject(history);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();            
        }
    }

    [Cmdlet(VerbsCommon.Add, nameof(TorrentHistory))]
    public class AddTorrentHistoryCmdlet : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public List<TorrentHistory> TorrentHistory { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Provider { get; set; }

        [Parameter]
        public string SeriesName { get; set; }

        [Parameter]
        public int? SeasonNumber { get; set; }

        [Parameter]
        public int? EpisodeNumber { get; set; }

        [Parameter]
        public DateTime? AirDate { get; set; }

        [Parameter]
        public DateTime? Date { get; set; } = DateTime.Now;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (TorrentHistory != null)
                foreach (TorrentHistory t in TorrentHistory)
                    MB.uTorrent.TorrentHistory.AddTorrentHistory(t);
            else
            {
                MB.uTorrent.TorrentHistory.AddTorrentHistory(new TorrentHistory(Name, Provider, SeriesName, SeasonNumber, EpisodeNumber, AirDate, Date));
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }

    [Cmdlet(VerbsCommon.Remove, nameof(TorrentHistory))]
    public class RemoveTorrentHistoryCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public List<TorrentHistory> TorrentHistory { get; set; }
        
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            foreach (TorrentHistory t in TorrentHistory)
                MB.uTorrent.TorrentHistory.RemoveTorrentHistory(t);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }
}
