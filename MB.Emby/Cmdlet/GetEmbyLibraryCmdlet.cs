using EmbyClient.Dotnet.Model;
using MB.Emby.Api;
using MB.Emby.Model;
using MB.Metadata.Text;
using System.Management.Automation;

namespace MB.Emby.Cmdlet;

[Cmdlet(VerbsCommon.Get, "EmbyLibrary")]
[OutputType(typeof(Library))]
public class GetEmbyLibraryCmdlet : EmbyItemCmdlet
{
    private Dictionary<ItemQueryParam, string> defaultQueryParam = new()
    {
        { ItemQueryParam.sortBy, "Name" },
        { ItemQueryParam.sortOrder, "Ascending" },
        { ItemQueryParam.recursive, "false" }
    };

    protected override void BeginProcessing()
    {
        base.BeginProcessing();
        this.InitializeClient(defaultQueryParam);
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        QueryResultBaseItemDto queryResult = service!.GetItems(queryParam);

        if (queryResult.TotalRecordCount > 0)
        {
            foreach (var item in queryResult.Items)
                if (TextUtils.TestSimpleMatch(item.Name, Name, true))
                    WriteObject(new Library(item));
        }
    }
}
