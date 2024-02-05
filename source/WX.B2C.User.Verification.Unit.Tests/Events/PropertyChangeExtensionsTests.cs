using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Extensions;

namespace WX.B2C.User.Verification.Unit.Tests.Events
{
    public class TinTestModel
    {
        public TinType Type { get; set; }

        public string Number { get; set; }
    }

    public class PropertyChangeExtensionsTests
    {
        [Test]
        public void ShouldFindPropertyChange()
        {
            // Arrange
            var previousValue = new TinDto { Type = TinType.ITIN, Number = "12345" };
            var newValue = new TinDto { Type = TinType.SSN, Number = "67890" };
            var serialized = new PropertyChangeDto
            {
                PropertyName = XPathes.Tin,
                NewValue = PropertyChangeSerializer.Serialize(newValue),
                PreviousValue = PropertyChangeSerializer.Serialize(previousValue)
            };

            // Act
            var tinChange = new[] { serialized }.Find<TinTestModel>(XPathes.Tin);

            // Assert
            tinChange.NewValue.Should()
                     .BeEquivalentTo(newValue, opt => opt.ComparingByMembers<TinDto>());
            tinChange.PreviousValue.Should()
                     .BeEquivalentTo(previousValue, opt => opt.ComparingByMembers<TinDto>());
        }
    }
}
