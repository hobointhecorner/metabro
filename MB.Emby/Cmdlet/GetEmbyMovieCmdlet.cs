using MB.Emby.Api;
using MB.Metadata.Text;
using System.Management.Automation;

namespace MB.Emby.Cmdlet;

[Cmdlet(VerbsCommon.Get, "EmbyMovie")]
[OutputType(typeof(Model.Movie))]
public class GetEmbyMovieCmdlet : EmbyItemCmdlet
{
    private Dictionary<ItemQueryParam, string> defaultQueryParam = new()
    {
        { ItemQueryParam.includeItemTypes, "Movie" },
        { ItemQueryParam.sortBy, "SortName" },
        { ItemQueryParam.recursive, "true" },
        { ItemQueryParam.limit, "500" },
        { ItemQueryParam.fields, "Overview,DateCreated,OfficialRating,PremiereDate,ProductionYear,RunTimeTicks,Path,Status" }
    };

    protected override void BeginProcessing()
    {
        base.BeginProcessing();
        this.InitializeClient(defaultQueryParam);
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        var queryResult = service!.GetItems(queryParam);

        if (queryResult.TotalRecordCount > 0)
        {
            foreach (var item in queryResult.Items)
                if (TextUtils.TestSimpleMatch(item.Name, Name))
                    WriteObject(new Model.Movie(item));
        }
    }
}
