using MB.Metadata.Text;
using System.Management.Automation;

namespace MB.Rss.Cmdlet;

[Cmdlet(VerbsCommon.Get, "RssProvider")]
[OutputType(typeof(Provider))]
public class GetRssProviderCmdlet : PSCmdlet
{
    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string[] Name { get; set; } = new string[] { "*" };

    [Parameter]
    [ValidateNotNullOrEmpty]
    public string Type { get; set; } = "*";

    internal List<Provider>? providerList;

    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        providerList = Provider.GetProvider();
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        foreach (string name in Name)
        {
            var output = from Provider provider in providerList!
                         where TextUtils.TestSimpleMatch(provider.Name, name) &&
                               TextUtils.TestSimpleMatch(provider.Type!, Type)
                         orderby provider.Name
                         select provider;

            foreach (Provider provider in output)
            {
                WriteObject(provider);
            }
        }        
    }
}
