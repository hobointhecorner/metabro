using System.Management.Automation;

namespace MB.Rss.Cmdlet;

[Cmdlet(VerbsCommon.Add, "RssProvider")]
[OutputType(typeof(Provider))]
public class AddRssProviderCmdlet : PSCmdlet
{
    [Parameter(Position = 0, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string? Name { get; set; }

    [Parameter(Position = 1, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string? Url { get; set; }

    [Parameter]
    [ValidateSet("feed", "disabled")]
    public string? Type { get; set; } = "feed";

    [Parameter]
    [ValidateNotNullOrEmpty]
    public int? Priority { get; set; } = 100;

    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? RootKey { get; set; }

    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? TitleAttr { get; set; }

    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? LinkAttr { get; set; }

    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? PubdateAttr { get; set; }

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

        var providerNames = from provider in providerList
                            select provider.Name;

        if (!providerNames.Contains(Name))
        {
            Provider provider = new(Name!, Url!, Type!, (int)Priority!);

            if (RootKey != null) provider.RootKey = RootKey;
            if (TitleAttr != null) provider.TitleAttr = TitleAttr;
            if (LinkAttr != null) provider.LinkAttr = LinkAttr;
            if (PubdateAttr != null) provider.PubdateAttr = PubdateAttr;

            providerList!.Add(provider);
            Provider.WriteProvider(providerList);

            if (PassThru)
            {
                WriteObject(provider);
            }
        }
        else
        {
            WriteWarning($"Provider with name {Name} already exists");
        }
    }
}
