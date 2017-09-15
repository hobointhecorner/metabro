using MB.TextParsing.QualityRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MB.TextParsing.QualityRules
{
    public static class DownloadQualityRule
    {
        public static string SubDirectory = "TextParsing";
        public static string FileName = "DownloadQualityRules.json";

        public static List<QualityRule> GetQualityRule()
        {
            List<QualityRule> qualityRule = QualityRule.GetQualityRule(DownloadQualityRule.FileName, DownloadQualityRule.SubDirectory);
            return qualityRule;
        }

        public static List<QualityRule> GetQualityRule(String Type)
        {
            return (from rule in QualityRule.GetQualityRule(DownloadQualityRule.FileName, DownloadQualityRule.SubDirectory)
                    where rule.Type.ToLower() == Type.ToLower()
                    select rule).ToList<QualityRule>();
        }

        public static int GetDownloadQuality(string Name, string Provider = null, string SeriesName = null, int? SeasonNumber = null, int? EpisodeNumber = null, DateTime? AirDate = null)
        {
            int quality = 0;

            List<QualityRule> ruleList = DownloadQualityRule.GetQualityRule();

            if (SeriesName != null)
                quality += 50000;

            if (EpisodeNumber != null || AirDate != null)
                quality += 50000;

            if (ruleList != null)
            {
                if (Provider != null)
                {
                    foreach (QualityRule rule in from QualityRule r in ruleList where r.Type.ToLower() == "provider" select r)
                        quality += QualityRule.TestQualityRule(rule, Provider);
                }

                foreach (QualityRule rule in from QualityRule r in ruleList where r.Type.ToLower() != "provider" select r)
                    quality += QualityRule.TestQualityRule(rule, Name);
            }

            return quality;
        }

        public static int GetDownloadQuality(Dictionary<string,string> ObjectInfo)
        {
            int? season = null;
            int? episode = null;
            DateTime? airDate = null;

            if (ObjectInfo["SeasonNumber"] != null)
                season = Convert.ToInt32(ObjectInfo["SeasonNumber"]);

            if (ObjectInfo["EpisodeNumber"] != null)
                season = Convert.ToInt32(ObjectInfo["EpisodeNumber"]);

            if (ObjectInfo["DateTime"] != null)
                airDate = DateTime.Parse(ObjectInfo["AirDate"]);


            return GetDownloadQuality(ObjectInfo["Name"], ObjectInfo["Provider"], ObjectInfo["SeriesName"], season, episode, airDate);
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(DownloadQualityRule))]
    [OutputType(typeof(DownloadQualityRule))]
    public class GetDownloadQualityRuleCmdlet : Cmdlet
    {
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            List<QualityRule> qualityRule = QualityRule.GetQualityRule(DownloadQualityRule.FileName, DownloadQualityRule.SubDirectory);
            if (qualityRule != null)
                foreach (QualityRule rule in qualityRule)
                    WriteObject(rule);
        }
    }

    [Cmdlet(VerbsCommon.Add, nameof(DownloadQualityRule))]
    public class AddDownloadQualityRuleCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Value { get; set; }

        [Parameter(Mandatory = true)]
        public string Type { get; set; }

        [Parameter(Mandatory = true)]
        public int Points { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            QualityRule.AddQualityRule(new QualityRule(Value, Type, Points), DownloadQualityRule.FileName, DownloadQualityRule.SubDirectory);
        }
    }

    [Cmdlet(VerbsCommon.Remove, nameof(DownloadQualityRule))]
    public class RemoveDownloadQualityRuleCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public QualityRule Rule { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            QualityRule.RemoveQualityRule(Rule, DownloadQualityRule.FileName, DownloadQualityRule.SubDirectory);
        }
    }

    [Cmdlet(VerbsCommon.Get, "DownloadQuality")]
    public class GetDownloadQualityCmdlet : Cmdlet
    {
        [Parameter (ValueFromPipeline = true)]
        public Dictionary<string,string> ObjectInfo { get; set; }

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
        public List<QualityRule> QualityRule { get; set; } = DownloadQualityRule.GetQualityRule();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (ObjectInfo != null)
                WriteObject(DownloadQualityRule.GetDownloadQuality(ObjectInfo));
            else if (Name != null)
                WriteObject(DownloadQualityRule.GetDownloadQuality(Name, Provider, SeriesName, SeasonNumber, EpisodeNumber, AirDate));
        }
    }
}
