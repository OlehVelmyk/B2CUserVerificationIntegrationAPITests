using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;

namespace WX.B2C.User.Verification.Integration.Tests.Steps;

internal class StepDecorator : IStep
{
    private readonly IStep _subject;
    private readonly IList<IAdditionalCondition> _preConditions = new List<IAdditionalCondition>();
    private readonly IList<IAdditionalCondition> _postConditions = new List<IAdditionalCondition>();

    private StepDecorator(IStep subject)
    {
        _subject = subject;
    }

    public static StepDecorator Create(IStep step) =>
        new(step);

    public StepDecorator AddPrecondition(IAdditionalCondition condition)
    {
        _preConditions.Add(condition);
        return this;
    }

    public StepDecorator AddPostCondition(IAdditionalCondition condition)
    {
        _postConditions.Add(condition);
        return this;
    }

    public Task Init() =>
        _subject.Init();

    public Task Execute() =>
        _subject.Execute();

    public async Task PreCondition()
    {
        foreach (var condition in _preConditions)
            await condition.Execute();
        await _subject.PreCondition();
    }

    public async Task PostCondition()
    {
        await _subject.PostCondition();
        foreach (var condition in _postConditions)
            await condition.Execute();
    }
}
