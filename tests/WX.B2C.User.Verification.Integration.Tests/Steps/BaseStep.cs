using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;

namespace WX.B2C.User.Verification.Integration.Tests.Steps;

internal abstract class BaseStep : IStep
{
    public virtual Task Init()
    {
        return Task.CompletedTask;
    }

    public virtual Task PreCondition()
    {
        return Task.CompletedTask;
    }

    public virtual Task Execute()
    {
        return Task.CompletedTask;
    }

    public virtual Task PostCondition()
    {
        return Task.CompletedTask;
    }
}
