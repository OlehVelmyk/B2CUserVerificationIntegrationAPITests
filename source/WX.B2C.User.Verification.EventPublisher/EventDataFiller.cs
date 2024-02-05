using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.EventPublisher.Mappers;
using WX.B2C.User.Verification.Events.Dtos;
using WX.B2C.User.Verification.Events.Enums;
using WX.B2C.User.Verification.Events.Events;
using WX.B2C.User.Verification.Extensions;
using WX.Messaging.Core;

namespace WX.B2C.User.Verification.EventPublisher
{
    internal interface IEventDataFiller
    {
        Task<Event> FillAsync(Event @event);
    }

    internal class EventDataFiller : IEventDataFiller
    {
        private readonly IProfileStorage _profileStorage;
        private readonly IVerificationDetailsDtoMapper _verificationDetailsDtoMapper;

        public EventDataFiller(IProfileStorage profileStorage, IVerificationDetailsDtoMapper verificationDetailsDtoMapper)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _verificationDetailsDtoMapper = verificationDetailsDtoMapper ?? throw new ArgumentNullException(nameof(verificationDetailsDtoMapper));
        }

        public Task<Event> FillAsync(Event @event) =>
            @event switch
            {
                VerificationDetailsUpdatedEvent detailsUpdatedEvent => FillAsync(detailsUpdatedEvent),
                _                                                   => Task.FromResult(@event)
            };

        private async Task<Event> FillAsync(VerificationDetailsUpdatedEvent @event)
        {
            var eventArgs = @event.EventArgs;

            var verificationDetails = await _profileStorage.GetVerificationDetailsAsync(eventArgs.UserId);
            eventArgs.VerificationDetails = _verificationDetailsDtoMapper.Map(verificationDetails);
            return @event;
        }
    }
}
