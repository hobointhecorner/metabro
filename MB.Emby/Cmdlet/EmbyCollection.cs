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
    public class EmbyCollection : EmbyItem
    {
        public EmbyCollection(string Id)
        {
            this.Id = Id;
        }

        public EmbyCollection(Api.EmbyCollection ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
        }        

        public static List<EmbyCollection> GetCollection(Dictionary<string,string> Parameter, ApiClient ApiClient = null)
        {
            List<EmbyCollection> collectionList = new List<EmbyCollection>();
            foreach (Api.EmbyCollection apiCollectionObject in ApiClient.GetCollection(Parameter))
                collectionList.Add(new EmbyCollection(apiCollectionObject));

            return collectionList;
        }

        public List<EmbyCollectionItem> GetCollectionItem(Dictionary<string, string> Parameter = null, ApiClient ApiClient = null)
        {
            List<EmbyCollectionItem> CollectionItemList = new List<EmbyCollectionItem>();

            if (ApiClient == null)
                ApiClient = new ApiClient();

            if (Parameter == null)
            {
                Parameter = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Series);
                Parameter["ParentId"] = Id;
                Parameter["Limit"] = null;
            }

            foreach (Api.EmbyCollectionItem apiCollectionItemObject in ApiClient.GetCollectionItem(Parameter))
                CollectionItemList.Add(new EmbyCollectionItem(apiCollectionItemObject));

            return CollectionItemList;
        }
    }

    public class EmbyCollectionItem : EmbyItem
    {
        public EmbyCollectionItem(Api.EmbyCollectionItem ApiObject)
        {
            this.Id = ApiObject.Id;
            this.Name = ApiObject.Name;
            this.Type = ApiObject.Type;
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyCollection))]
    [OutputType(typeof(EmbyCollection))]
    public class GetEmbyCollectionCmdlet : PSCmdlet
    {
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; } = "*";

        Dictionary<string, string> RequestParam = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Collection);
        ApiClient ApiClient = new ApiClient();

        protected override void BeginProcessing()
        {
            base.BeginProcessing();            
        }
        
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            foreach (EmbyCollection collectionObject in EmbyCollection.GetCollection(RequestParam, ApiClient))
            {
                if (TextParser.TestSimpleMatch(Name, collectionObject.Name))
                    WriteObject(collectionObject);
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbyCollectionItem))]
    [OutputType(typeof(EmbyCollectionItem))]
    public class GetEmbyCollectionItemCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public List<EmbyCollection> Collection { get; set; }

        Dictionary<string, string> RequestParam = ApiClient.GetRequestParameter(ApiClient.RequestParameterType.Series);
        ApiClient ApiClient = new ApiClient();

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            RequestParam["Limit"] = null;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            foreach (EmbyCollection collectionObject in Collection)
            {
                RequestParam["ParentId"] = collectionObject.Id;
                foreach (EmbyCollectionItem collectionItem in collectionObject.GetCollectionItem(RequestParam, ApiClient))
                    WriteObject(collectionItem);
            }
        }
    }
}
