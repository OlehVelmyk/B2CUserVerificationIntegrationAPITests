using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Public
{
    internal partial class ActionsTests
    {
        private async Task ShouldGetActions(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var client = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var (expectedActions, expectedSurveys) = ActionsFixture.GetExpectedActions(userInfo);

            // Act
            var userActions = await client.Actions.GetAsync();
            var actualActions = userActions.Select(ua => ua.ActionType);

            // Assert
            actualActions.Should().BeEquivalentTo(expectedActions);
            foreach (var survey in expectedSurveys)
                userActions.Should().ContainsSurvey(survey.id, survey.tag);
        }
    }
}
