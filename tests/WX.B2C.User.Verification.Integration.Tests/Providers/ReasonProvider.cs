using System.Runtime.CompilerServices;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Providers;

internal static class ReasonProvider
{
    public static string Create(string? reason = null, [CallerMemberName] string? callerMethod = null) =>
        Generate(reason, callerMethod!);

    public static ReasonDto CreateDto(string? reason = null, [CallerMemberName] string? callerMethod = null) =>
        new(Generate(reason, callerMethod!));

    private static string Generate(string? providedReason, string callerMethod) =>
        $"{providedReason ?? ""} due to End to End testing, caller {callerMethod}";
}
