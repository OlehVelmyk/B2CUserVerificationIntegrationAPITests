using FluentAssertions;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Integration.Tests.Providers;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminRequestSurveyStep : BaseStep
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly IPublicClient _publicApiClient;
    private readonly SurveyType _surveyType;
    private readonly TaskType _targetTaskType;

    public AdminRequestSurveyStep(VerificationAdminApiClientFactory adminApiClientFactory,
                                  IPublicClient publicApiClient,
                                  SurveyType surveyType, 
                                  TaskType targetTaskType)
    {
        _adminApiClientFactory = adminApiClientFactory;
        _publicApiClient = publicApiClient;
        _surveyType = surveyType;
        _targetTaskType = targetTaskType;
    }

    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();
        
        var tasks = await adminApiClient.Tasks.GetAllAsync(userId);
        var targetTask = tasks.First(t => t.Type == _targetTaskType);
        var request = new SurveyCollectionStepRequest
        {
            Type = CollectionStepType.Survey,
            TemplateId = SurveyHelper.GetSurveyId(_surveyType),
            IsRequired = true,
            IsReviewNeeded = true,
            TargetTasks = new List<Guid> { targetTask.Id },
            Reason = ReasonProvider.Create(callerMethod: nameof(AdminRequestSurveyStep))
        };

        await adminApiClient.CollectionStep.RequestAsync(request, userId);
    }

    public override async Task PostCondition()
    {
        var userAction = new UserAction(ActionType.Survey, surveyType: _surveyType);
        var actions = await _publicApiClient.ExecuteUntilAsync(
            client => client.Actions.GetAsync(),
            actions => actions.Any(userAction.Equals));

        actions.Should().ContainSingle(action => userAction.Equals(action), $"After period of time action for survey: {_surveyType} is not open");
    }
}

internal class AdminRequestSurveyStepFactory
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly IPublicClient _publicApiClient;

    public AdminRequestSurveyStepFactory(VerificationAdminApiClientFactory adminApiClientFactory,
                                         IPublicClient publicApiClient)
    {
        _adminApiClientFactory = adminApiClientFactory;
        _publicApiClient = publicApiClient;
    }

    public AdminRequestSurveyStep Create(SurveyType surveyType, TaskType targetTaskType) =>
        new(_adminApiClientFactory, _publicApiClient, surveyType, targetTaskType);
}
