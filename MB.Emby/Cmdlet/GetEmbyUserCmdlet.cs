using MB.Emby.Api;
using MB.Emby.Model;
using MB.Metadata.Text;
using System.Management.Automation;

namespace MB.Emby.Cmdlet;

[Cmdlet(VerbsCommon.Get, "EmbyUser")]
[OutputType(typeof(User))]
[CmdletBinding(DefaultParameterSetName = "query")]
public class GetEmbyUserCmdlet : EmbyUserCmdlet
{
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string Name { get; set; } = "*";

    [Parameter]
    public string[]? Id { get; set; } = null;

    [Parameter]
    public bool? Hidden { get; set; } = null;

    [Parameter]
    public bool? Disabled { get; set; } = null;

    [Parameter]
    public int? Limit { get; set; } = null;

    private Dictionary<UserQueryParam, string> defaultQueryParam = new();

    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        if (Hidden != null) defaultQueryParam.Add(UserQueryParam.isHidden, Hidden.ToString()!);
        if (Disabled != null) defaultQueryParam.Add(UserQueryParam.isDisabled, Disabled.ToString()!);
        if (Limit != null) defaultQueryParam.Add(UserQueryParam.limit, Limit.ToString()!);

        this.InitializeClient(defaultQueryParam);
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        if (Id != null)
        {
            foreach (string userId in Id)
            {
                var userObject = service!.GetUser(userId);
                
                if (userObject != null)
                    WriteObject(new User(userObject));
            }
        }
        else
        {
            var queryResult = service!.GetUsers(defaultQueryParam);

            if (queryResult.TotalRecordCount > 0)
                foreach (var user in queryResult.Items)
                    if (TextUtils.TestSimpleMatch(user.Name, Name))
                        WriteObject(new User(user));
        }
    }
}
