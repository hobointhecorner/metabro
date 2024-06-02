using MB.Metadata.Text;
using System.Management.Automation;

namespace MB.TextParsing.Cmdlet;

[Cmdlet(VerbsCommon.Add, "WeightRule")]
[OutputType(typeof(WeightRule))]
public class AddWeightRuleCmdlet : PSCmdlet
{
    [Parameter(Position=0, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string? Value { get; set; }

    [Parameter(Position=1, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public int? Weight { get; set; }

    [Parameter(Position = 2)]
    [ValidateNotNullOrEmpty]
    public WeightRuleType Type { get; set; } = WeightRuleType.Other;

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    IEnumerable<WeightRule> ruleList;

    public AddWeightRuleCmdlet()
    {
        ruleList = WeightRule.GetRules();
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        if (!WeightRule.Exists(Value!, ruleList))
        {
            WeightRule newRule = new(Type, Value!, (int)Weight!);

            ruleList = ruleList.Append(newRule);
            if (PassThru) WriteObject(newRule);
        }
        else
        {
            WriteWarning($"Rule with Value {Value} already exists.  Use Set-WeightRule to modify it.");
        }
    }

    protected override void EndProcessing()
    {
        base.EndProcessing();

        WeightRule.WriteRules(ruleList);
    }
}
