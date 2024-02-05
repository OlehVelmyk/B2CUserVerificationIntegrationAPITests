namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;

public class BaseFlow : IFlow
{
    private readonly List<IStep> _steps = new();

    public IFlow AddStep(IStep step)
    {
        _steps.Add(step);
        return this;
    }

    public async Task Execute()
    {
        foreach (var step in _steps)
        {
            await step.Init();
            await step.PreCondition();
            await step.Execute();
            await step.PostCondition();
        }
    }
}
