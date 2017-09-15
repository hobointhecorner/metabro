using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MB.TextParsing.QualityRules
{
    public static class FileQualityRule
    {
        public static string SubDirectory = "TextParsing";
        public static string FileName = "FileQualityRules.json";

        public static List<QualityRule> GetQualityRule()
        {
            List<QualityRule> qualityRule = QualityRule.GetQualityRule(FileQualityRule.FileName, FileQualityRule.SubDirectory);
            return qualityRule;
        }

        public static List<QualityRule> GetQualityRule(String Type)
        {
            return (from rule in QualityRule.GetQualityRule(FileQualityRule.FileName, FileQualityRule.SubDirectory)
                    where rule.Type.ToLower() == Type.ToLower()
                    select rule).ToList<QualityRule>();
        }

        public static int GetFileQuality(string FileName, List<QualityRule> RuleList)
        {
            int quality = 0;
            foreach (QualityRule rule in RuleList)
                quality += QualityRule.TestQualityRule(rule, FileName);

            return quality;
        }
    }

    [Cmdlet(VerbsCommon.Get, nameof(FileQualityRule))]
    [OutputType(typeof(FileQualityRule))]
    public class GetFileQualityRuleCmdlet : Cmdlet
    {
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            List<QualityRule> qualityRule = FileQualityRule.GetQualityRule();
            if (qualityRule != null)
                foreach (QualityRule rule in qualityRule)
                    WriteObject(rule);
        }
    }

    [Cmdlet(VerbsCommon.Add, nameof(FileQualityRule))]
    public class AddFileQualityRuleCmdlet : Cmdlet
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
            QualityRule.AddQualityRule(new QualityRule(Value, Type, Points), FileQualityRule.FileName, FileQualityRule.SubDirectory);
        }
    }

    [Cmdlet(VerbsCommon.Remove, nameof(FileQualityRule))]
    public class RemoveFileQualityRuleCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public QualityRule Rule { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            QualityRule.RemoveQualityRule(Rule, FileQualityRule.FileName, FileQualityRule.SubDirectory);
        }
    }
}
