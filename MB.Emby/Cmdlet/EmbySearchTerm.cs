using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MB.File;
using MB.TextParsing;

namespace MB.Emby
{
    public class EmbySearchTerm
    {
        public string Id { get; set; }
        public string SeriesId { get; set; }
        public string SeriesName { get; set; }
        public string SearchTerm { get; set; }

        public EmbySearchTerm()
        {

        }

        public EmbySearchTerm(string SeriesId, string SeriesName, string SearchTerm)
        {
            this.SeriesId = SeriesId;
            this.SeriesName = SeriesName;
            this.SearchTerm = SearchTerm;
            this.Id = Guid.NewGuid().ToString();
        }

        public EmbySearchTerm(string SeriesId, string SeriesName, string SearchTerm, string Id)
        {
            this.SeriesId = SeriesId;
            this.SeriesName = SeriesName;
            this.SearchTerm = SearchTerm;
            this.Id = Id;
        }

        public static Dictionary<string, string> Export(List<EmbySearchTerm> SearchTerm)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach (EmbySearchTerm term in SearchTerm)
                output.Add(term.SearchTerm, term.SeriesName);
            
            return output;
        }

        public static string GetSubDirectory()
        {
            return "Emby";
        }

        public static string GetFileName()
        {
            return "SearchTerms.json";
        }

        public static IEnumerable<EmbySearchTerm>GetSearchTerm()
        {
            return FileProvider.GetFile<List<EmbySearchTerm>>(GetFileName(), GetSubDirectory());
        }

        public static void WriteSearchTerm(List<EmbySearchTerm>SearchTerm)
        {
            FileProvider.WriteFile<List<EmbySearchTerm>>(SearchTerm, GetFileName(), GetSubDirectory());
        }

        public static void AddSearchTerm(EmbySearchTerm SearchTerm)
        {
            List<EmbySearchTerm> searchTermList = (List<EmbySearchTerm>)EmbySearchTerm.GetSearchTerm();

            if (searchTermList != null)
                searchTermList.Add(SearchTerm);
            else
            {
                searchTermList = new List<EmbySearchTerm>();
                searchTermList.Add(SearchTerm);
            }

            WriteSearchTerm(searchTermList);
        }

        public static void RemoveSearchTerm(EmbySearchTerm SearchTerm)
        {
            List<EmbySearchTerm> searchTermList = EmbySearchTerm.GetSearchTerm().ToList<EmbySearchTerm>();
            if (searchTermList != null)
            {
                var outputList = from term in searchTermList
                                 where term.Id != SearchTerm.Id
                                 select term;

                WriteSearchTerm(outputList.ToList<EmbySearchTerm>());
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(EmbySearchTerm))]
    [OutputType(typeof(List<EmbySearchTerm>))]
    public class GetEmbySearchTermCmdlet : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public EmbySeries Series { get; set; }

        [Parameter(Position = 0)]
        public string SearchTerm { get; set; } = "*";

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var searchTermList = EmbySearchTerm.GetSearchTerm();
            if (Series != null)
            {
                searchTermList = from searchTerm in searchTermList
                                 where searchTerm.SeriesId == Series.Id
                                 select searchTerm;
            }

            if (searchTermList != null)
                foreach (EmbySearchTerm searchTerm in searchTermList)
                    if (TextParser.TestSimpleMatch(SearchTerm, searchTerm.SearchTerm))
                        WriteObject(searchTerm);
        }
    }

    [Cmdlet(VerbsCommon.Add, nameof(EmbySearchTerm))]
    public class AddEmbySearchTermCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public EmbySeries Series { get; set; }

        [Parameter(Position = 0, Mandatory = true)]
        public string SearchTerm { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            string seriesName = Series.Name;
            string seriesId = Series.Id;
            EmbySearchTerm.AddSearchTerm(new EmbySearchTerm(seriesId, seriesName, SearchTerm));
        }
    }

    [Cmdlet(VerbsCommon.Remove, nameof(EmbySearchTerm))]
    [OutputType(typeof(List<EmbySearchTerm>))]
    public class RemoveEmbySearchTermCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public EmbySearchTerm SearchTerm { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            EmbySearchTerm.RemoveSearchTerm(SearchTerm);
        }
    }
}
