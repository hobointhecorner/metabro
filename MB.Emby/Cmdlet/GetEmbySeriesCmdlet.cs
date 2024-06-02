using MB.Emby.Api;
using MB.Emby.Model;
using MB.Metadata.Text;
using System.Management.Automation;

namespace MB.Emby.Cmdlet;

[Cmdlet(VerbsCommon.Get, "EmbySeries")]
[OutputType(typeof(Model.Series))]
public class GetEmbySeriesCmdlet : EmbyItemCmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public string[]? ParentId { get; set; } = null;

    public bool? Recurse { get; set; } = null;

    private Dictionary<ItemQueryParam, string> defaultQueryParam = new()
    {
        { ItemQueryParam.includeItemTypes, "Series" },
        { ItemQueryParam.sortBy, "SortName" },
        { ItemQueryParam.limit, "500" },
        { ItemQueryParam.fields, "Overview,DateCreated,OfficialRating,PremiereDate,ProductionYear,RunTimeTicks,Status,Path" }
    };

    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        if (Recurse == null)
            if (ParentId == null) Recurse = true;
            else Recurse = false;

        this.defaultQueryParam.Add(ItemQueryParam.recursive, Recurse.ToString()!);

        this.InitializeClient(defaultQueryParam);
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        if (ParentId != null)
        {
            foreach (string itemId in ParentId)
            {
                if (queryParam.ContainsKey(ItemQueryParam.parentId))
                    queryParam[ItemQueryParam.parentId] = itemId;
                else
                    queryParam.Add(ItemQueryParam.parentId, itemId);

                var queryResult = service!.GetItems(queryParam);

                if (queryResult.TotalRecordCount > 0)
                {
                    foreach (var item in queryResult.Items)
                        if (TextUtils.TestSimpleMatch(item.Name, Name))
                            WriteObject(new Model.Series(item));
                }
            }
        }
        else
        {
            var queryResult = service!.GetItems(queryParam);

            if (queryResult.TotalRecordCount > 0)
            {
                foreach (var item in queryResult.Items)
                    if (TextUtils.TestSimpleMatch(item.Name, Name))
                        WriteObject(new Model.Series(item));
            }
        }        
    }
}
