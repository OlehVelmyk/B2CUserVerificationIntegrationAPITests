namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;

public interface IStep
{
    Task Init();

    Task PreCondition();

    Task Execute();

    Task PostCondition();
}
