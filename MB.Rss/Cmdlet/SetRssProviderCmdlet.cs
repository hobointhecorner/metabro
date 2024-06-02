using System.Management.Automation;

namespace MB.Rss.Cmdlet;

[Cmdlet(VerbsCommon.Set, "RssProvider")]
[OutputType(typeof(Provider))]
public class SetRssProviderCmdlet : PSCmdlet
{
    [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    [ValidateNotNullOrEmpty]
    public string[]? Name { get; set; }

    [Parameter]
    public string? Url { get; set; } = null;

    [Parameter]
    [ValidateSet("feed", "disabled")]
    public string? Type { get; set; }

    [Parameter]
    public int? Priority { get; set; }

    [Parameter]
    public string? RootKey { get; set; }

    [Parameter]
    public string? TitleAttr { get; set; }

    [Parameter]
    public string? LinkAttr { get; set; }

    [Parameter]
    public string? PubdateAttr { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

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
            var currentProvider = (
                from Provider provider in providerList!
                where provider.Name == name
                select provider
            ).First<Provider>();

            if (Url != null) currentProvider.Url = Url;
            if (Type != null) currentProvider.Type = Type;
            if (Priority != null) currentProvider.Priority = (int)Priority;
            if (RootKey != null) currentProvider.RootKey = RootKey;
            if (TitleAttr != null) currentProvider.TitleAttr = TitleAttr;
            if (LinkAttr != null) currentProvider.LinkAttr = LinkAttr;
            if (PubdateAttr != null) currentProvider.PubdateAttr = PubdateAttr;

            var updatedList = from Provider provider in providerList!
                              where provider.Name != name
                              select provider;

            providerList = updatedList.ToList();
            providerList.Add(currentProvider);

            if (PassThru) WriteObject(currentProvider);
        }
    }

    protected override void EndProcessing()
    {
        base.EndProcessing();

        Provider.WriteProvider(providerList!);
    }
}
