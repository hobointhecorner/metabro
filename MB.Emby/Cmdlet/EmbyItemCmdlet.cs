using MB.Emby.Api;
using MB.Emby.Model;
using System.Management.Automation;

namespace MB.Emby.Cmdlet;

public class EmbyItemCmdlet : EmbyCmdlet
{
    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string Name { get; set; } = "*";

    [Parameter]
    public string[]? Id { get; set; }

    [Parameter]
    public int? Limit { get; set; }

    [Parameter]
    public User? User { get; set; }

    private protected ItemService? service;
    private protected Dictionary<ItemQueryParam, string> queryParam = new();

    private protected void SetQueryParams(Dictionary<ItemQueryParam, string> param)
    {
        if (User == null && embyPref!.DefaultUserId != null) 
            queryParam.Add(ItemQueryParam.userId, embyPref.DefaultUserId);

        foreach (ItemQueryParam key in param.Keys)
        {
            if (param.ContainsKey(key)) queryParam[key] = param[key];
            else queryParam.Add(key, param[key]);
        }
    }

    private new protected void InitializeClient()
    {
        base.InitializeClient();

        service = new ItemService(apiConfig!);

        if (Id != null) queryParam.Add(ItemQueryParam.ids, String.Join(',', Id));
        if (Limit != null) queryParam.Add(ItemQueryParam.limit, Limit.ToString()!);
        if (User != null) queryParam.Add(ItemQueryParam.userId, User.Id);
    }

    private protected void InitializeClient(Dictionary<ItemQueryParam, string> defaultParam)
    {
        this.InitializeClient();
        SetQueryParams(defaultParam);
    }
}
