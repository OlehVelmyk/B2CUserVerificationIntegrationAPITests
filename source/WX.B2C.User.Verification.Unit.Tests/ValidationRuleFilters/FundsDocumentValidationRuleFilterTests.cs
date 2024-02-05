using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Services.Validation;

namespace WX.B2C.User.Verification.Unit.Tests.ValidationRuleFilters
{
    [TestFixture]
    public class FundsDocumentValidationRuleFilterTests
    {
        private FundsDocumentValidationRuleFilter _filter;
        private IUserSurveyProvider _surveyProvider;
    
        [SetUp]
        public void Setup()
        {
            _surveyProvider = Substitute.For<IUserSurveyProvider>();
            _filter = new FundsDocumentValidationRuleFilter(_surveyProvider);
        }

        [Theory]
        public async Task ApplyAsync_ShouldReturnEmptyDocumentValidationRule_WhenAnswersNotFround()
        {
            // arrange
            var validationRuleDto = new DocumentValidationRuleDto
            {
                AllowedTypes = new Dictionary<string, DocumentTypeValidationRuleDto>
                {
                    {"some_allowed_types", new DocumentTypeValidationRuleDto()}
                }
            };
            var context = new ValidationRuleFilterContext()
            {
                Country = "GB",
                ActionType = ActionType.ProofOfFunds,
                UserId = Guid.NewGuid()
            };
            _surveyProvider
                .GetAnswersAsync(context.UserId, Arg.Any<Guid>(), Arg.Any<IList<string>>())
                .ReturnsForAnyArgs(new List<TaggedAnswer>(0));
        
            // act
            var result = await _filter.ApplyAsync(validationRuleDto, context);
        
            // assert
            var documentValidationRuleDto = result as DocumentValidationRuleDto;
            documentValidationRuleDto.Should().NotBeNull();
            documentValidationRuleDto?.AllowedTypes.Keys.Count.Should().Be(0, "AllowedTypes should be empty");
        }
    }
}

