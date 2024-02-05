using System;
using System.Collections.Generic;
using WX.B2C.User.Profile.Events.Dtos;
using WX.B2C.User.Profile.Events.EventArgs;
using WX.B2C.User.Profile.Events.Events;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Builders
{
    internal class ProfileEventsBuilder
    {
        private readonly UserProfileDto _profile = new();
        private readonly Guid _correlationId;

        private readonly IDictionary<string, Action<UserProfileDto, UserInfo>> _actions =
            new Dictionary<string, Action<UserProfileDto, UserInfo>>
            {
                [nameof(PersonalDetails.FirstName)] = (profile, userInfo) => profile.FirstName = userInfo.FirstName,
                [nameof(PersonalDetails.LastName)] = (profile, userInfo) => profile.LastName = userInfo.LastName,
                [nameof(PersonalDetails.Birthdate)] = (profile, userInfo) => profile.DateOfBirth = userInfo.DateOfBirth,
                [nameof(PersonalDetails.Nationality)] = (profile, userInfo) => profile.Nationality = userInfo.Nationality,
                [nameof(PersonalDetails.Email)] = (profile, userInfo) => profile.Email = userInfo.Email,
                [nameof(PersonalDetails.ResidenceAddress)] = (profile, userInfo) => profile.ResidenceAddress = userInfo.Address.SafeMap(),
                [nameof(PersonalDetails.FullName)] = (_, _) => { }
            };

        public ProfileEventsBuilder(Guid? correlationId = null)
        {
            _correlationId = correlationId ?? Guid.NewGuid();
        }

        public ProfileEventsBuilder With(UserInfo userInfo, PersonalDetailsPatch patch)
        {
            _profile.UserId = userInfo.UserId;
            foreach (var property in patch.Properties)
                SetProperty(userInfo, property);

            return this;
        }

        public UserProfileUpdatedEvent BuildProfileUpdatedEvent() =>
            new(
                new UserProfileEventArgs
                {
                    CorrelationId = _correlationId,
                    UserProfile = _profile
                }, _correlationId);


        public UserProfileCreatedEvent BuildProfileCreatedEvent() =>
            new(
                new UserProfileEventArgs
                {
                    CorrelationId = _correlationId,
                    UserProfile = _profile
                }, _correlationId);

        private void SetProperty(UserInfo userInfo, PersonalDetailsProperty property)
        {
            if (!_actions.TryGetValue(property.Type, out var action))
                throw new ArgumentOutOfRangeException(nameof(property.Type), property.Type, "Unsupported property type.");

            action(_profile, userInfo);
        }
    }
}