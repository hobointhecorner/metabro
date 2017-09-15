using MB.Emby.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby
{
    public class EmbyEpisode : EmbyItem
    {
        public string SeriesId { get; set; }
        public string SeriesName { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public DateTime AirDate { get; set; }
        public string Year { get; set; }
        public bool IsHD { get; set; }

        public EmbyEpisode(Api.EmbyEpisode ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
            this.SeriesId = ApiObject.SeriesId;
            this.SeriesName = ApiObject.SeriesName;
            this.SeasonNumber = ApiObject.ParentIndexNumber;
            this.EpisodeNumber = ApiObject.IndexNumber;
            this.Year = ApiObject.ProductionYear;
            this.IsHD = ApiObject.IsHD;

            if (ApiObject.PremiereDate != null)
                this.AirDate = DateTime.Parse(ApiObject.PremiereDate);
        }

        public EmbyEpisode(Api.EmbyEpisodeDetail ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
            this.SeriesId = ApiObject.SeriesId;
            this.SeriesName = ApiObject.SeriesName;
            this.SeasonNumber = ApiObject.ParentIndexNumber;
            this.EpisodeNumber = ApiObject.IndexNumber;
            this.Year = ApiObject.ProductionYear;
            this.IsHD = ApiObject.IsHD;

            if (ApiObject.PremiereDate != null)
                this.AirDate = DateTime.Parse(ApiObject.PremiereDate);
        }

        public static List<EmbyEpisode> GetEpisode(Dictionary<string, string> Parameter, ApiClient ApiClient = null)
        {
            List<EmbyEpisode> EpisodeList = new List<EmbyEpisode>();
            if (ApiClient == null)
                ApiClient = new ApiClient();

            foreach (Api.EmbyEpisode apiEpisodeObject in ApiClient.GetEpisode(Parameter))
                EpisodeList.Add(new EmbyEpisode(apiEpisodeObject));

            return EpisodeList;
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyEpisode))]
    [OutputType(typeof(EmbyEpisode))]
    public class GetEmbyEpisodeCmdlet : PSCmdlet
    {
        [Parameter(ParameterSetName = "Series", Mandatory = true, ValueFromPipeline = true)]
        public List<EmbySeries> Series { get; set; }

        [Parameter(ParameterSetName = "Library", Mandatory = true, ValueFromPipeline = true)]
        public List<EmbyLibrary> Library { get; set; }

        [Parameter (ParameterSetName = "Id")]
        public string Id { get; set; }

        [Parameter(ParameterSetName = "Series")]
        [Parameter(ParameterSetName = "Library")]
        public string UserId { get; set; }
        
        [Parameter(ParameterSetName = "Series")]
        [Parameter(ParameterSetName = "Library")]
        public SwitchParameter Missing { get; set; }

        [Parameter(ParameterSetName = "Series")]
        [Parameter(ParameterSetName = "Library")]
        public SwitchParameter Unaired { get; set; }

        [Parameter(ParameterSetName = "Series")]
        [Parameter(ParameterSetName = "Library")]
        [Parameter(ParameterSetName = "Id")]
        public int? Limit { get; set; }

        List<EmbyEpisode> episodeList = new List<EmbyEpisode>();
        Dictionary<string, string> requestParam = new Dictionary<string, string>();
        ApiClient ApiClient = new ApiClient();

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (Id != string.Empty && UserId == string.Empty)
                UserId = ApiClient.Preferences.DefaultUserId;

            if (Id != null)
                throw new NotImplementedException("Episode by ID is not supported yet");
            else
                requestParam = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Episode);

            if (Missing || Unaired)
            {
                requestParam["IsMissing"] = "True";
                requestParam["SortBy"] = "PremiereDate";
                requestParam["SortOrder"] = "Descending";

                if (Limit == null)
                    requestParam["Limit"] = null;
            }
            else if (Library != null)
            {
                requestParam["SortBy"] = "SeriesName,ParentIndexNumber,IndexNumber";
            }

            if (Limit != null)
                if (Limit == 0)
                    requestParam["Limit"] = null;
                else
                    requestParam["Limit"] = Limit.ToString();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Series != null)
            {
                if (Missing || Unaired)
                {
                    throw new Exception("Cannot search for missing or unaired episodes by series due to Emby API limitations.");
                }

                foreach (EmbySeries seriesObject in Series)                
                    episodeList.AddRange(seriesObject.GetEpisode(requestParam, ApiClient));                
            }
            else if (Library != null)
            {
                foreach (EmbyLibrary libraryObject in Library)
                {
                    requestParam["ParentId"] = libraryObject.Id;
                    episodeList.AddRange(EmbyEpisode.GetEpisode(requestParam));
                }
            }
            else if (Id != null)
            {
                requestParam["Id"] = Id;
                requestParam["UserId"] = UserId;
                episodeList.AddRange(EmbyEpisode.GetEpisode(requestParam));
            }
            else
            {
                if (Missing != null || Unaired != null)
                    requestParam["SortBy"] = "SeriesName,ParentIndexNumber,IndexNumber";

                foreach (EmbyEpisode episodeObject in EmbyEpisode.GetEpisode(requestParam, ApiClient))
                    episodeList.Add(episodeObject);
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            IEnumerable<EmbyEpisode> episodeOutputList;

            if (Unaired)
            {
                episodeOutputList = from episode in episodeList
                              where episode.AirDate >= DateTime.Now
                              select episode;
            }
            else
            {
                episodeOutputList = from episode in episodeList
                                        where episode.AirDate <= DateTime.Now
                                        select episode;
            }

            foreach (EmbyEpisode episodeObject in episodeOutputList)
            {
                WriteObject(episodeObject);
            }
        }
    }
}
