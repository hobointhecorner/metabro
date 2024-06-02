using MB.Metadata.Text;
using System.Management.Automation;

namespace MB.TextParsing.Cmdlet;

[Cmdlet(VerbsCommon.Remove, "WeightRule")]
[OutputType(typeof(WeightRule))]
public class RemoveWeightRuleCmdlet : PSCmdlet
{
    [Parameter(Position=0, Mandatory = true, ValueFromPipeline = true)]
    [ValidateNotNullOrEmpty]
    public WeightRule[]? Rule { get; set; }

    IEnumerable<WeightRule> ruleList;

    public RemoveWeightRuleCmdlet()
    {
        ruleList = WeightRule.GetRules();
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        foreach (var removeRule in Rule!)
        {
            ruleList = from rule in ruleList
                       where rule.Id != removeRule.Id
                       select rule;
        }
    }

    protected override void EndProcessing()
    {
        base.EndProcessing();

        WeightRule.WriteRules(ruleList);
    }
}
