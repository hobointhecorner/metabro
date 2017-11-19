using MB.Emby;
using MB.TextParsing;
using MB.TextParsing.QualityRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MB.RSS
{
    public class RssTorrent
    {
        public string Name { get; set; }
        public string SeriesName { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
        public DateTime? AirDate { get; set; }
        public string Url { get; set; }
        public DateTime? DatePublished { get; set; }
        public string Provider { get; set; }
        public int Quality = 0;

        public RssTorrent()
        {

        }

        public override string ToString()
        {
            return this.Url;
        }

        public RssTorrent(XmlNode XmlInfo, RssTorrentProvider Provider)
        {
            ParseRss(XmlInfo, Provider);
        }
        
        public void ParseRss(XmlNode XmlInfo, RssTorrentProvider Provider)
        {
            foreach (XmlNode n in XmlInfo.ChildNodes)
            {
                if (n.Name.ToLower() == "title")
                    this.Name = n.InnerText;

                else if (n.Name.ToLower() == "link")
                    this.Url = n.InnerText;

                else if (n.Name.ToLower() == "pubdate")
                    this.DatePublished = DateTime.Parse(n.InnerText);
            }

            this.Provider = Provider.Name;
        }

        public static List<RssTorrent> GetTorrent(RssTorrentProvider RssProvider, bool FillInfo = false, List<EmbySeries> SeriesList = null)
        {
            List<RssTorrent> output = new List<RssTorrent>();
            
            XmlDocument result = RssClient.GetRssFeed(RssProvider.Url);

            foreach (XmlNode rssItem in result.SelectNodes("rss/channel/item"))
            {
                RssTorrent torrent = new RssTorrent(rssItem, RssProvider);
                if (FillInfo)
                    torrent.FillInfoFromEmby(SeriesList);

                output.Add(torrent);
            }

            return output;
        }

        public void FillInfoFromEmby(List<EmbySeries>SeriesList = null)
        {
            if (SeriesList == null)
            {
                Dictionary<string, string> requestParam = Emby.Api.ApiClient.GetRequestParameter(Emby.Api.ApiClient.RequestParameterType.Series);
                requestParam["Limit"] = null;
                requestParam["Recursive"] = "true";
                SeriesList = EmbySeries.GetSeries(requestParam);
            }

            List<string> seriesNameList = (from EmbySeries series in SeriesList select series.Name).ToList();
            List<EmbySearchTerm> searchTermList = EmbySearchTerm.GetSearchTerm().ToList();

            string seriesName = TextParser.GetSeriesName(Name, seriesNameList, EmbySearchTerm.Export(searchTermList));
            if (seriesName != null)
                SeriesName = seriesName;

            SeasonEpisode seasonEpisode = TextParser.GetSeasonEpisodeNumber(Name);
            DateTime? airDate = TextParser.GetAirDate(Name);
            if (seasonEpisode != null)
            {
                this.SeasonNumber = seasonEpisode.Season;
                this.EpisodeNumber = seasonEpisode.Episode[0];
            }
            else if (airDate != null)
            {
                this.AirDate = airDate;
            }

            this.Quality = DownloadQualityRule.GetDownloadQuality(Name, Provider, SeriesName, SeasonNumber, EpisodeNumber, AirDate);
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(RssTorrent))]
    [OutputType(typeof(List<RssTorrent>))]
    public class GetRssTorrentCmdlet : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public List<RssTorrentProvider> Provider { get; set; } = RssTorrentProvider.GetProvider();

        [Parameter]
        public SwitchParameter FillInfo { get; set; } = true;
        
        [Parameter]
        public SwitchParameter VerifyInfo { get; set; } = true;

        [Parameter]
        public List<EmbySeries> SeriesList { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (FillInfo && SeriesList == null)
            {
                Dictionary<string, string> requestParam = Emby.Api.ApiClient.GetRequestParameter(Emby.Api.ApiClient.RequestParameterType.Series);
                requestParam["Limit"] = null;
                requestParam["Recursive"] = "true";
                SeriesList = EmbySeries.GetSeries(requestParam);
            }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            List<RssTorrent> torrentList = new List<RssTorrent>();

            if (Provider != null)
                foreach (RssTorrentProvider provider in Provider)
                    foreach (RssTorrent torrent in RssTorrent.GetTorrent(provider, FillInfo, SeriesList))
                        if (VerifyInfo)
                        {
                            if (torrent.SeriesName != null && ((torrent.SeasonNumber != null && torrent.EpisodeNumber != null) || torrent.AirDate != null))
                                WriteObject(torrent);                            
                        }
                        else
                            WriteObject(torrent);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }

    [Cmdlet(VerbsCommon.Find, nameof(RssTorrent))]
    [OutputType(typeof(List<RssTorrent>))]
    public class FindRssTorrentCmdlet : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public List<RssTorrent> Torrent { get; set; }

        [Parameter]
        public String SeriesName { get; set; }

        [Parameter]
        public int? SeasonNumber { get; set; }

        [Parameter]
        public int? EpisodeNumber { get; set; }

        [Parameter]
        public DateTime? AirDate { get; set; }
        
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (Torrent != null)
            { 
                if (SeriesName != null)
                {
                    Torrent = (from t in Torrent
                               where TextParser.TestSimpleMatch(SeriesName, t.SeriesName)
                               select t).ToList();
                }

                if (SeasonNumber != null)
                {
                    Torrent = (from t in Torrent
                               where t.SeasonNumber == SeasonNumber
                               select t).ToList();
                }

                if (EpisodeNumber != null)
                {
                    Torrent = (from t in Torrent
                               where t.EpisodeNumber == EpisodeNumber
                               select t).ToList();
                }

                if (AirDate != null)
                {
                    Torrent = (from t in Torrent
                               where t.AirDate == AirDate
                               select t).ToList();
                }

                if (Torrent != null)
                    foreach (RssTorrent t in Torrent)
                        WriteObject(t);
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }
}
