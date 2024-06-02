using MB.File;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MB.Metadata.Text;

public enum WeightRuleType
{
    Uploader,
    Quality,
    Other
}

public class WeightRule
{
    public string Value { get; set; }
    public int Weight { get; set; }
    public WeightRuleType Type { get; set; }
    public Guid Id { get; set; }
    
    protected int SortValue
    {
        get
        {
            PropertyInfo ruleProp = this.GetType().GetProperty(defaultSort)!;

            return (int)ruleProp.GetValue(this)!;
        }
    }

    private static readonly string filename = "weight_rules.json";
    private static readonly string subdirectory = "metadata";
    private static readonly string defaultSort = "Weight";

    private static readonly Dictionary<WeightRuleType, List<(string value, int weight)>> defaultRules = new()
    {
        {
            WeightRuleType.Quality, new List<(string value, int weight)>
            {
                ( value: "1080p", weight: 5000 ),
                ( value: "1440p", weight: 4000 ),
                ( value: "web",   weight: 3000 ),
                ( value: "720p",  weight: 2000 ),
                ( value: "htdv",  weight: 1000 ),
                ( value: "480p",  weight: -50000 )
            }
        },
        {
            WeightRuleType.Other, new List<(string value, int weight)>
            {
                ( value: "proper", weight: 500 ),
                ( value: "repack", weight: 500 )
            }
        }
    };

    public WeightRule(WeightRuleType type, string value, int weight)
    {
        this.Type = type;
        this.Value = value;
        this.Weight = weight;
        this.Id = Guid.NewGuid();
    }

    // Filter rules based on input variables
    bool Filter(string? value = null, WeightRuleType? ruleType = null)
    {
        bool matchValue = value == null || TextUtils.TestSimpleMatch(this.Value, value!);
        bool matchType = ruleType == null || ruleType == this.Type;

        return matchValue && matchType;
    }

    // Get a list of default weight rules
    static List<WeightRule> GetDefaultRules()
    {
        List<WeightRule> output = new();

        foreach (var ruleDict in defaultRules)
        {
            var ruleType = ruleDict.Key;
            var ruleList = from ruleTup in ruleDict.Value
                           let rule = new WeightRule(ruleType, ruleTup.value, ruleTup.weight)
                           select rule;

            output.AddRange(ruleList);
        }

        return output;
    }

    // Create rules file if it doesn't already exist
    static void InitializeRules()
    {
        if (!FileProvider.TestFile(filename, subdirectory))
        {
            var defaultRules = GetDefaultRules();
            WriteRules(defaultRules);
        }
    }

    // Sort rules based on value in defaultSort
    static List<WeightRule> SortRules(IEnumerable<WeightRule> rules)
    {
        return (from rule in rules
                orderby rule.SortValue
                select rule).ToList();
    }

    public static List<WeightRule> GetRules(string? value = null, WeightRuleType? ruleType = null)
    {
        InitializeRules();

        var ruleList = from rule in FileProvider.GetFile<List<WeightRule>>(filename, subdirectory)
                       where rule.Filter(value, ruleType)
                       select rule;

        return SortRules(ruleList);
    }

    public static void WriteRules(IEnumerable<WeightRule> rules)
    {
        var ruleList = SortRules(rules);
        FileProvider.WriteFile<List<WeightRule>>(ruleList, filename, subdirectory);
    }

    public static bool Exists(string value, IEnumerable<WeightRule> rules)
    {
        var duplicateList = from rule in rules
                            where value.ToLower() == rule.Value.ToLower()
                            select rule;

        return duplicateList.Any();
    }

    public int GetWeight(string input)
    {
        var regex = new System.Text.RegularExpressions.Regex(Value, RegexOptions.IgnoreCase);
        if (regex.IsMatch(input))
        {
            return Weight;
        }
        else
        {
            return 0;
        }
    }
}
