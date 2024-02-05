using System;
using System.Threading.Tasks;
using WX.B2C.User.Profile.Events.Dtos;
using WX.B2C.User.Profile.Events.EventArgs;
using WX.B2C.User.Profile.Events.Events;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class ProfileFixture
    {
        private readonly EventsFixture _eventsFixture;

        public ProfileFixture(EventsFixture eventsFixture)
        {
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public async Task CreateAsync(UserInfo userInfo)
        {
            if (userInfo is null) 
                throw new ArgumentNullException(nameof(userInfo));

            var userProfileDto = Map(userInfo);
            var userProfileEventArgs = new UserProfileEventArgs
            {
                UserProfile = userProfileDto,
                CorrelationId = Guid.NewGuid()
            };

            await _eventsFixture.PublishAsync(new UserProfileUpdatedEvent(userProfileEventArgs));
            _eventsFixture.ShouldExist<PersonalDetailsUpdatedEvent>(userProfileEventArgs.CorrelationId);
        }

        public async Task UpdateAsync(Guid userId, Address address)
        {
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            var userProfileEventArgs = new UserProfileEventArgs
            {                
                UserProfile = new UserProfileDto 
                { 
                    UserId = userId,
                    ResidenceAddress = address.SafeMap()
                },
                CorrelationId = Guid.NewGuid()
            };

            await _eventsFixture.PublishAsync(new UserProfileUpdatedEvent(userProfileEventArgs));
            _eventsFixture.ShouldExist<PersonalDetailsUpdatedEvent>(userProfileEventArgs.CorrelationId);
        }


        private static UserProfileDto Map(UserInfo userInfo) =>
            new()
            {
                UserId = userInfo.UserId,
                Email = userInfo.Email,
                DateOfBirth = userInfo.DateOfBirth,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Nationality = userInfo.Nationality,
                ResidenceAddress = userInfo.Address.SafeMap()
            };
    }
}