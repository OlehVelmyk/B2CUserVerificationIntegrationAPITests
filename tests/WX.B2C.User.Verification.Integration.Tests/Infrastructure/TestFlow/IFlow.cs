namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;

public interface IFlow
{
    IFlow AddStep(IStep step);

    Task Execute();
}
