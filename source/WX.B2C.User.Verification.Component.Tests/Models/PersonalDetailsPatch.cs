using System;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class PersonalDetailsPatch
    {
        public PersonalDetailsProperty[] Properties { get; set; }
    }

    internal class PersonalDetailsProperty
    {
        private readonly Func<PersonalDetailsDto, object> _actualValueSelector;
        private readonly Func<UserInfo, PersonalDetailsDto, object> _expectedValueSelector;
        private readonly Func<PropertyChangeDto[], PropertyChange<object>> _changeSelector;

        public string Type { get; }

        public PersonalDetailsProperty(string type,
                                       Func<PersonalDetailsDto, object> actualValueSelector,
                                       Func<UserInfo, PersonalDetailsDto, object> expectedValueSelector,
                                       Func<PropertyChangeDto[], PropertyChange<object>> changeSelector)
        {
            Type = type;
            _actualValueSelector = actualValueSelector ?? throw new ArgumentNullException(nameof(actualValueSelector));
            _expectedValueSelector = expectedValueSelector ?? throw new ArgumentNullException(nameof(expectedValueSelector));
            _changeSelector = changeSelector ?? throw new ArgumentNullException(nameof(changeSelector));
        }

        public object GetActualValue(PersonalDetailsDto personalDetailsDto) =>
            _actualValueSelector(personalDetailsDto);

        public object GetExpectedValue(UserInfo userInfo, PersonalDetailsDto personalDetailsDto) =>
            _expectedValueSelector(userInfo, personalDetailsDto);

        public PropertyChange<object> GetChange(PropertyChangeDto[] changes) =>
            _changeSelector(changes);
    }
}