using System;
using System.Threading.Tasks;
using WX.B2C.Survey.Events;
using WX.B2C.Survey.Events.Dtos;
using WX.B2C.Survey.Events.EventArgs;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class SurveyFixture
    {
        private readonly EventsFixture _eventsFixture = new EventsFixture();

        public async Task SubmitAsync(Guid userId, Guid templateId, Guid? correlationId = null)
        {
            var args = new UserSurveySubmittedEventArgs()
            {
                UserId = userId,
                SurveyId = templateId,
                Answers = Array.Empty<AnswerDto>()
            };
            var @event = new UserSurveySubmittedEvent(userId.ToString(), args, Guid.NewGuid(), correlationId);
            await _eventsFixture.PublishAsync(@event);
        }
    }
}
