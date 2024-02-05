using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Models;

internal class UserCheck : IEquatable<UserCheck>
{
    public CheckType Type { get; }

    public CheckProviderType Provider { get; }

    public CheckState State { get; }

    public CheckResult? Result { get; }

    public string? Decision { get; }

    public UserCheck(CheckType type,
                     CheckProviderType provider,
                     CheckState state,
                     CheckResult? result,
                     string? decision)
    {
        Type = type;
        Provider = provider;
        State = state;
        Result = result;
        Decision = decision;
    }

    public static UserCheck Create(CheckType type,
                                   CheckProviderType provider = CheckProviderType.System,
                                   CheckState state = CheckState.Complete,
                                   CheckResult? result = CheckResult.Passed,
                                   string? decision = null) =>
        new(type, provider, state, result, decision);

    public bool Equals(UserCheck? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Type == other.Type && Provider == other.Provider && State == other.State && Result == other.Result && Decision == other.Decision;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return Equals((UserCheck) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int) Type, (int) Provider, (int) State, Result, Decision);
    }

    public static bool operator ==(UserCheck? left, UserCheck? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(UserCheck? left, UserCheck? right)
    {
        return !Equals(left, right);
    }
    
    public override string ToString()
    {
        return $"{nameof(Type)}: {Type}, {nameof(Provider)}: {Provider}, " +
               $"{nameof(State)}: {State}, {nameof(Result)}: {Result}, " +
               $"{nameof(Decision)} : {Decision ?? "null"}";
    }
}
