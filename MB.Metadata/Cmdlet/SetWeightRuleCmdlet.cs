using MB.Metadata.Text;
using System.Management.Automation;

namespace MB.TextParsing.Cmdlet;

[Cmdlet(VerbsCommon.Set, "WeightRule")]
[OutputType(typeof(WeightRule))]
public class SetWeightRuleCmdlet : PSCmdlet
{
    [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
    [ValidateNotNullOrEmpty]
    public List<WeightRule>? Rule { get; set; }

    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? Value { get; set; } = null;

    [Parameter]
    [ValidateNotNullOrEmpty]
    public int? Weight { get; set; } = null;

    [Parameter]
    [ValidateNotNullOrEmpty]
    public WeightRuleType? Type { get; set; } = null;

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    IEnumerable<WeightRule> ruleList;

    public SetWeightRuleCmdlet()
    {
        ruleList = WeightRule.GetRules();
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        foreach (var setRule in Rule!)
        {
            // Make desired changes
            if (Value != null)  setRule.Value = Value;
            if (Weight != null) setRule.Weight = (int)Weight;
            if (Type != null)   setRule.Type = (WeightRuleType)Type;

            // Remove changed rule
            ruleList = from rule in ruleList
                       where rule.Id != setRule.Id
                       select rule;

            // Append changed rule
            ruleList = ruleList.Append(setRule);

            if (PassThru) WriteObject(setRule);
        }
    }

    protected override void EndProcessing()
    {
        base.EndProcessing();

        WeightRule.WriteRules(ruleList.ToList());
    }
}
