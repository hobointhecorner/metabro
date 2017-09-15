using MB.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MB.TextParsing.QualityRules
{
    public class QualityRule
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }
        
        public QualityRule(string Value, string Type, int Points)
        {
            this.Value = Value;
            this.Type = Type;
            this.Points = Points;
        }

        public static List<QualityRule> GetQualityRule(string FileName, string SubDirectory)
        {
            return FileProvider.GetFile<List<QualityRule>>(FileName, SubDirectory);
        }

        public static void WriteQualityRule(List<QualityRule> QualityRule, string FileName, string SubDirectory)
        {
            FileProvider.WriteFile<List<QualityRule>>(QualityRule, FileName, SubDirectory);
        }

        public static void AddQualityRule(QualityRule NewRule, string FileName, string SubDirectory)
        {
            List<QualityRule> ruleList = GetQualityRule(FileName, SubDirectory);
            ruleList.Add(NewRule);

            WriteQualityRule(ruleList, FileName, SubDirectory);
        }

        public static void RemoveQualityRule(QualityRule Rule, string FileName, string SubDirectory)
        {
            List<QualityRule> ruleList = GetQualityRule(FileName, SubDirectory);
            var filterList = from r in ruleList
                             where r.Value != Rule.Value
                             select r;

            WriteQualityRule(filterList.ToList<QualityRule>(), FileName, SubDirectory);
        }

        public static int TestQualityRule(QualityRule Rule, string Text)
        {            
            if (new Regex(Rule.Value.ToLower()).IsMatch(Text.ToLower()))
                return Rule.Points;
            else
                return 0;
        }
    }


}
