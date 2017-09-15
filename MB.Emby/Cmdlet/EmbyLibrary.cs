using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MB.Emby.Api;
using MB.TextParsing;

namespace MB.Emby
{
    public class EmbyLibrary : EmbyItem
    {
        public EmbyLibrary(string LibraryId)
        {
            this.Id = LibraryId;
        }

        public EmbyLibrary(Api.EmbyLibrary ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
        }        

        public static List<EmbyLibrary> GetLibrary(Dictionary<string,string> Parameter, ApiClient ApiClient = null)
        {
            List<EmbyLibrary> OutputList = new List<EmbyLibrary>();
            if (ApiClient == null)
                ApiClient = new ApiClient();

            foreach (Api.EmbyLibrary apiLibraryObject in ApiClient.GetLibrary(Parameter))
                OutputList.Add(new EmbyLibrary(apiLibraryObject));

            return OutputList;
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyLibrary))]
    [OutputType(typeof(EmbyLibrary))]
    public class GetEmbyLibraryCmdlet : PSCmdlet
    {
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; } = "*";

        Dictionary<string, string> requestParam = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Library);
        ApiClient ApiClient = new ApiClient();

        protected override void BeginProcessing()
        {
            base.BeginProcessing();            
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            foreach (EmbyLibrary libraryObject in EmbyLibrary.GetLibrary(requestParam))
                if (TextParser.TestSimpleMatch(Name, libraryObject.Name))
                    WriteObject(libraryObject);
        }
    }
}
