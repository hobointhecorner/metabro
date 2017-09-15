using MB.Emby.Api;
using MB.TextParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MB.Emby
{
    public class EmbyMovie : EmbyItem
    {
        public bool IsHD { get; set; }
        public string OfficialRating { get; set; }
        public DateTime PremiereDate { get; set; }
        public string ProductionYear { get; set; }
        public TimeSpan RunTime { get; set; }

        public EmbyMovie(string Id)
        {
            this.Id = Id;
        }

        public EmbyMovie(Api.EmbyMovie ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
            this.OfficialRating = ApiObject.OfficialRating;
            this.ProductionYear = ApiObject.ProductionYear;

            if (ApiObject.IsHD != null)
            {
                if (ApiObject.IsHD.ToLower() == "true")
                    IsHD = true;
                else
                    IsHD = false;
            }

            if (ApiObject.RunTimeTicks != 0)
                RunTime = TimeSpan.FromTicks(ApiObject.RunTimeTicks);
        }

        public static List<EmbyMovie> GetMovie(Dictionary<string, string> Parameter, ApiClient ApiClient = null)
        {
            List<EmbyMovie> outputList = new List<EmbyMovie>();
            if (ApiClient == null)
                ApiClient = new ApiClient();

            foreach (Api.EmbyMovie apiMovieObject in ApiClient.GetMovie(Parameter))
                outputList.Add(new EmbyMovie(apiMovieObject));

            return outputList;
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyMovie))]
    [OutputType(typeof(EmbyMovie))]
    public class GetEmbyMovieCmdlet : PSCmdlet
    {
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; } = "*";

        [Parameter]
        public string Id { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public List<EmbyLibrary> Library { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public List<EmbyCollection> Collection { get; set; }

        public string Limit { get; set; }

        ApiClient ApiClient = new ApiClient();
        Dictionary<string, string> requestParam = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Movie);
        List<EmbyMovie> OutputList = new List<EmbyMovie>();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Library != null)
            {
                foreach (EmbyLibrary libraryObject in Library)
                {
                    requestParam["ParentId"] = libraryObject.Id;
                    OutputList.AddRange(EmbyMovie.GetMovie(requestParam, ApiClient));
                }
            }
            else if (Collection != null)
            {
                foreach (EmbyCollection collectionObject in Collection)
                {
                    requestParam["ParentId"] = collectionObject.Id;
                    OutputList.AddRange(EmbyMovie.GetMovie(requestParam, ApiClient));
                }
            }
            else
            {
                OutputList.AddRange(EmbyMovie.GetMovie(requestParam, ApiClient));
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            foreach (EmbyMovie movieObject in OutputList)
                if (TextParser.TestSimpleMatch(Name, movieObject.Name))
                    WriteObject(movieObject);
        }
    }
}
