using EmbyClient.Dotnet.Client;
using EmbyClient.Dotnet.Model;

namespace MB.Emby.Api;

internal class UserService : EmbyClient.Dotnet.Api.UserServiceApi
{
    public UserService() : base() { }
    public UserService(Configuration apiConfig) : base(apiConfig) { }

    internal static string? GetUserQueryParam(UserQueryParam queryParam, Dictionary<UserQueryParam, string> param)
    {
        if (param.ContainsKey(queryParam)) return param[queryParam];
        else return null;
    }

    internal static string GetUserQueryStringParam(UserQueryParam queryParam, Dictionary<UserQueryParam, string> param)
    {
        string? paramValue = GetUserQueryParam(queryParam, param);

        if (paramValue != null) return param[queryParam];
        else return String.Empty;
    }

    internal static bool? GetUserQueryBoolParam(UserQueryParam queryParam, Dictionary<UserQueryParam, string> param, bool isNullable = true)
    {
        bool parseResult;
        string? paramValue = GetUserQueryParam(queryParam, param);

        if (paramValue == null)
            if (isNullable) return null;
            else return false;
        else if (bool.TryParse(paramValue, out parseResult)) return parseResult;
        else throw new InvalidCastException($"Could not parse value '{paramValue}' as bool");
    }

    internal static int? GetUserQueryIntParam(UserQueryParam queryParam, Dictionary<UserQueryParam, string> param, bool isNullable = true)
    {
        int parseResult;
        string? paramValue = GetUserQueryParam(queryParam, param);

        if (paramValue == null)
            if (isNullable) return null;
            else return 0;
        else if (int.TryParse(paramValue, out parseResult)) return parseResult;
        else throw new InvalidCastException($"Could not parse value '{paramValue}' as int");
    }

    internal static double? GetUserQueryDoubleParam(UserQueryParam queryParam, Dictionary<UserQueryParam, string> param, bool isNullable = true)
    {
        double parseResult;
        string? paramValue = GetUserQueryParam(queryParam, param);

        if (paramValue == null)
            if (isNullable) return null;
            else return 0;
        else if (double.TryParse(paramValue, out parseResult)) return parseResult;
        else throw new InvalidCastException($"Could not parse value '{paramValue}' as double");
    }

    public UserDto GetUser(string id) => GetUsersById(id);

    public QueryResultUserDto GetUsers(Dictionary<UserQueryParam, string> userQueryParam)
    {
        return base.GetUsersQuery(
            GetUserQueryBoolParam(UserQueryParam.isHidden, userQueryParam),
            GetUserQueryBoolParam(UserQueryParam.isDisabled, userQueryParam),
            GetUserQueryIntParam(UserQueryParam.startIndex, userQueryParam),
            GetUserQueryIntParam(UserQueryParam.limit, userQueryParam),
            GetUserQueryStringParam(UserQueryParam.nameStartsWithOrGreater, userQueryParam)
        );
    }
}
