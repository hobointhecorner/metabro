using EmbyClient.Dotnet.Api;
using MB.Emby.Api;

namespace MB.Emby.Cmdlet;

public class EmbyUserCmdlet : EmbyCmdlet
{
    private protected UserService? service;
    private protected Dictionary<UserQueryParam, string> queryParam = new();

    private protected void SetQueryParams(Dictionary<UserQueryParam, string> param)
    {
        foreach (UserQueryParam key in param.Keys)
        {
            if (param.ContainsKey(key)) queryParam[key] = param[key];
            else queryParam.Add(key, param[key]);
        }
    }

    private new protected void InitializeClient()
    {
        base.InitializeClient();

        service = new UserService(apiConfig!);
    }

    private protected void InitializeClient(Dictionary<UserQueryParam, string> defaultParam)
    {
        this.InitializeClient();
        SetQueryParams(defaultParam);
    }
}
