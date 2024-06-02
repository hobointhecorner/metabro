using System.Management.Automation;

namespace MB.Rss.Cmdlet;

[Cmdlet(VerbsCommon.Remove, "RssProvider")]
[OutputType(typeof(Provider))]
public class RemoveRssProviderCmdlet : PSCmdlet
{
    [Parameter(Position=0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    [ValidateNotNullOrEmpty]
    public string[]? Name { get; set; }

    internal List<Provider>? providerList;

    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        providerList = Provider.GetProvider();
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        foreach (string name in Name!)
        {
            var filteredList = from provider in providerList
                               where provider.Name != name
                               select provider;

            providerList = filteredList.ToList<Provider>();
        }
    }

    protected override void EndProcessing()
    {
        base.EndProcessing();

        Provider.WriteProvider(providerList!);
    }
}
