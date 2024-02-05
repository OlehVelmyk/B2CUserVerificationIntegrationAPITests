using WX.Preconditions.Contracts;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;

public class BaseUserPreconditions : BasePreconditions<BaseUserPreconditions>
{
    public BaseUserPreconditions(string name) : base(name) { }
}