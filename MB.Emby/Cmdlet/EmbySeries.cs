using MB.Emby.Api;
using MB.TextParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MB.Emby
{
    public class EmbySeries : EmbyItem
    {
        public bool Ongoing { get; set; } = false;
        public string Year { get; set; }
        
        public EmbySeries(string Id)
        {
            this.Id = Id;
        }

        public EmbySeries(Api.EmbySeries ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
            this.Year = ApiObject.ProductionYear;

            if (ApiObject.Status != null && ApiObject.Status.ToLower() == "continuing")
            {
                this.Ongoing = true;
            }
        }
        
        public EmbySeries(Api.EmbyItem ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
        }        

        public static List<EmbySeries> GetSeries(Dictionary<string,string>Parameter = null, ApiClient ApiClient = null)
        {
            List<EmbySeries> OutputList = new List<EmbySeries>();
            if (Parameter == null)
            {
                Parameter = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Series);
                Parameter["Recursive"] = "true";
            }
            if (ApiClient == null)
                ApiClient = new ApiClient();

            foreach (Api.EmbySeries apiSeriesObject in ApiClient.GetSeries(Parameter))
                OutputList.Add(new EmbySeries(apiSeriesObject));

            return OutputList;
        }

        public static List<EmbySeries> GetSeries(string Id, Dictionary<string,string>Parameter = null, ApiClient ApiClient = null)
        {
            List<EmbySeries> OutputList = new List<EmbySeries>();
            if (Parameter == null)
                Parameter = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Series);

            Parameter["Ids"] = Id;

            return GetSeries(Parameter, ApiClient);
        }

        public static EmbySeries GetSeriesByName(string SeriesName, Dictionary<string, string> Parameter = null, ApiClient ApiClient = null)
        {
            List<EmbySeries> OutputList = new List<EmbySeries>();
            if (ApiClient == null)
                ApiClient = new ApiClient();

            if (Parameter == null)
                Parameter = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Series);

            Parameter["Recursive"] = "true";

            return (from EmbySeries series in EmbySeries.GetSeries(Parameter, ApiClient)
                    where series.Name == SeriesName
                    select series).FirstOrDefault();
        }

        public List<EmbySearchTerm>GetSearchTerms()
        {
            return EmbySearchTerm.GetSearchTerm().ToList();
        }

        public List<EmbyEpisode> GetEpisode(Dictionary<string,string> Parameter = null, ApiClient ApiClient = null)
        {
            if (Parameter == null)
                Parameter = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Episode);

            Parameter["ParentId"] = Id;

            return EmbyEpisode.GetEpisode(Parameter, ApiClient);
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbySeries))]
    [OutputType(typeof(EmbySeries))]
    public class GetEmbySeriesCmdlet : PSCmdlet
    {
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; } = "*";

        [Parameter]
        public string Id { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public List<EmbyLibrary>Library { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public List<EmbyCollection> Collection { get; set; }

        [Parameter]
        public string Limit { get; set; }

        ApiClient ApiClient = new ApiClient();
        Dictionary<string, string> requestParam = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Series);
        List<EmbySeries> OutputList = new List<EmbySeries>();

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (Name != "*")
                requestParam["Limit"] = null;
            else if (Limit == "0")
                requestParam["Limit"] = null;
            else if (Limit != null)
                requestParam["Limit"] = Limit;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Library != null)
            {
                foreach (EmbyLibrary LibraryObject in Library)
                {
                    requestParam["ParentId"] = LibraryObject.Id;
                    OutputList.AddRange(EmbySeries.GetSeries(requestParam, ApiClient));
                }
            }
            else if (Collection != null)
            {
                foreach (EmbyCollection collectionObject in Collection)
                {
                    requestParam["ParentId"] = collectionObject.Id;
                    OutputList.AddRange(EmbySeries.GetSeries(requestParam, ApiClient));
                }
            }
            else if (Id != null)
            {
                OutputList = EmbySeries.GetSeries(Id, requestParam, ApiClient);
            }
            else
            {
                requestParam["Recursive"] = "True";
                OutputList = EmbySeries.GetSeries(requestParam, ApiClient);
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            if (OutputList.Count > 0)
            {
                foreach (EmbySeries SeriesObject in OutputList)
                    if (TextParser.TestSimpleMatch(Name, SeriesObject.Name))
                        WriteObject(SeriesObject);
            }
            else
            {
                WriteWarning("No series records returned");
            }
        }
    }
}