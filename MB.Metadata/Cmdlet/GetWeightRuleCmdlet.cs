using MB.Metadata.Text;
using System.Management.Automation;

namespace MB.TextParsing.Cmdlet;

[Cmdlet(VerbsCommon.Get, "WeightRule")]
[OutputType(typeof(WeightRule))]
public class GetWeightRuleCmdlet : PSCmdlet
{
    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string? Value { get; set; } = "*";

    [Parameter]
    public WeightRuleType? WeightRuleType { get; set; } = null;

    IEnumerable<WeightRule> ruleList;

    public GetWeightRuleCmdlet()
    {
        ruleList = WeightRule.GetRules(Value, WeightRuleType);
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        ruleList = from rule in ruleList
                   where 
                      TextUtils.TestSimpleMatch(rule.Value, Value!)
                      &&
                      (
                          WeightRuleType == null
                          || rule.Type == WeightRuleType
                      )
                   select rule;

        foreach (var rule in ruleList)
        {
            WriteObject(rule);
        }
    }
}
