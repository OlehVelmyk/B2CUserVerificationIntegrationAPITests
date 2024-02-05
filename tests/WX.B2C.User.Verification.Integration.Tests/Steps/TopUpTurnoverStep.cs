using FluentAssertions;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.EventsService;
using WX.Integration.Tests.EventListener.Common;
using IAdminClient = WX.B2C.User.Verification.Api.Admin.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps;

internal class TopUpTurnoverStep : BaseStep
{
    private readonly VerificationAdminApiClientFactory _clientFactory;
    private readonly EventPublisher _eventPublisher;
    private readonly EventSink _eventSink;
    private readonly int _turnoverAmount;
    private readonly Guid _variantId;
    private readonly Guid _userId;
    private ManualResetEventSlim _triggerCompleted;

    public TopUpTurnoverStep(
        EventSink eventSink,
        VerificationAdminApiClientFactory clientFactory,
        EventPublisher eventPublisher,
        Guid variantId,
        int turnoverAmount)
    {
        _clientFactory = clientFactory;
        _eventSink = eventSink;
        _eventPublisher = eventPublisher;
        _variantId = variantId;
        _turnoverAmount = turnoverAmount;
        
        _userId = Guid.Parse(StepContext.Instance[General.UserId]);
    }

    public override async Task PreCondition()
    {
        var applicationId = await GetApplicationId(_userId);
        _triggerCompleted = _eventSink.SubscribeTo<TriggerCompletedEvent>(x =>
            x.EventArgs.UserId == _userId &&
            x.EventArgs.ApplicationId == applicationId &&
            x.EventArgs.VariantId == _variantId);
    }

    public override async Task Execute()
    {
        var @event = RiskAssessmentTransactionHelper.CreateTopUpTransactionEvent(
            _userId.ToString(),
            _turnoverAmount);
        await _eventPublisher.PublishAsync(@event);
    }

    public override Task PostCondition()
    {
        _triggerCompleted.Wait(Timeouts.VerificationWaitTimeout).Should().BeTrue(
            $"{nameof(TriggerCompletedEvent)} wasn't published. VariantId: {_variantId} Turnover amount: {_turnoverAmount}");

       return Task.CompletedTask;
    }

    private async Task<Guid> GetApplicationId(Guid userId)
    {
        var adminApiClient = await _clientFactory.Create();
        var application = await adminApiClient.Applications.GetDefaultAsync(userId);

        return application.Id;
    }
}

internal class TopUpTurnoverStepFactory
{
    private readonly EventSink _eventSink;
    private readonly EventPublisher _eventPublisher;
    private readonly VerificationAdminApiClientFactory _clientFactory;

    public TopUpTurnoverStepFactory(
        EventSink eventSink,
        VerificationAdminApiClientFactory clientFactory,
        EventPublisher eventPublisher)
    {
        _eventSink = eventSink;
        _clientFactory = clientFactory;
        _eventPublisher = eventPublisher;
    }

    public TopUpTurnoverStep Create(Guid variantId, int turnoverAmount) =>
        new(_eventSink, _clientFactory, _eventPublisher, variantId, turnoverAmount);
}
