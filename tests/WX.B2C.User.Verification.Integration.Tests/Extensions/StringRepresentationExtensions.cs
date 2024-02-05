using Newtonsoft.Json;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Extensions;

internal static class StringRepresentationExtensions
{
    public static string AsString(this IEnumerable<CheckDto> checks) =>
        string.Join("\n", checks.Select(AsString));

    public static string AsString(this CheckDto check) =>
        $"{{\n\t{nameof(CheckDto.Id)}: {check.Id},\n\t" +
        $"{nameof(CheckDto.Type)}: {check.Type},\n\t" +
        $"{nameof(CheckDto.State)}: {check.State},\n\t" +
        $"{nameof(CheckDto.Result)}: {check.Result},\n\t" +
        $"{nameof(CheckDto.Decision)}: {check.Decision}\n}}";

    public static string AsString(this IEnumerable<UserActionDto> actions) =>
        string.Join("\n", actions.Select(AsString));

    public static string AsString(this UserActionDto action) =>
        $"{{\n\t{nameof(UserActionDto.ActionType)}: {action.ActionType},\n\t" +
        $"{nameof(UserActionDto.Priority)}: {action.Priority},\n\t" +
        $"{nameof(UserActionDto.Reason)}: {action.Reason},\n\t" +
        $"{nameof(UserActionDto.IsOptional)}: {action.IsOptional},\n\t" +
        $"{nameof(UserActionDto.ActionData)}: {JsonConvert.SerializeObject(action.ActionData)}\n}}";
    
    public static string AsString(this IEnumerable<UserAction> actions) =>
        string.Join("\n", actions.Select(action => action.ToString()));
}
