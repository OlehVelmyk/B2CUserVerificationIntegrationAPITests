using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions;

namespace WX.B2C.User.Verification.Unit.Tests.InternalApi
{
    public class CollectionStepExtensionsTests
    {
        /// <summary>
        /// Given steps with same Xpath "1" but only one requested and not required
        /// And steps with same Xpath "2" but both completed and only one required
        /// And steps with same Xpath "3" but both requested and only one required (impossible situation according to domain logic)
        /// When called <see cref="CollectionStepExtensions.ToDictionary"/>
        /// Then returned dictionary must contain unique item by Xpath by priority where
        /// "1" requested and not required
        /// "2" completed and required
        /// "3" requested and required
        /// </summary>
        [Test]
        public void ToDictionary_ShouldReturnStepsByPriority()
        {
            var steps = new[]
            {
                new CollectionStepDto
                {
                    State = CollectionStepState.Completed,
                    XPath = "1",
                    IsRequired = true,
                },
                new CollectionStepDto
                {
                    State = CollectionStepState.Completed,
                    XPath = "2",
                    IsRequired = false,
                },
                new CollectionStepDto
                {
                    State = CollectionStepState.Requested,
                    XPath = "1",
                    IsRequired = false,
                },
                new CollectionStepDto
                {
                    State = CollectionStepState.Requested,
                    XPath = "3",
                    IsRequired = false,
                },
                new CollectionStepDto
                {
                    State = CollectionStepState.Completed,
                    XPath = "1",
                    IsRequired = false,
                },
                new CollectionStepDto
                {
                    State = CollectionStepState.Completed,
                    XPath = "2",
                    IsRequired = true,
                },
                new CollectionStepDto
                {
                    State = CollectionStepState.Requested,
                    XPath = "3",
                    IsRequired = true,
                }
            };

            var uniqueSteps = steps.ToDictionary();
            
            uniqueSteps.Should().HaveCount(3);
            uniqueSteps["1"]
                .Should()
                .Match<CollectionStepDto>(dto => dto.XPath == "1" && !dto.IsRequired && dto.State == CollectionStepState.Requested);
            uniqueSteps["2"]
                .Should()
                .Match<CollectionStepDto>(dto => dto.XPath == "2" && dto.IsRequired && dto.State == CollectionStepState.Completed);
            uniqueSteps["3"]
                .Should()
                .Match<CollectionStepDto>(dto => dto.XPath == "3" && dto.IsRequired && dto.State == CollectionStepState.Requested);
        }
    }
}