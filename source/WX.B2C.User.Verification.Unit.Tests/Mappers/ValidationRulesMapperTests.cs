using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Public.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Public.Services;

namespace WX.B2C.User.Verification.Unit.Tests.Mappers
{
    [TestFixture]
    public class ValidationRulesMapperTests
    {
        private IValidationRulesMapper _mapper;
        private DocumentValidationRuleDto _documentValidationRuleDto;
        
        [SetUp]
        public void Setup()
        {
            var documentCategoryMapper = new DocumentCategoryMapper();
            _mapper = new ValidationRulesMapper(documentCategoryMapper);
            _documentValidationRuleDto = new DocumentValidationRuleDto()
            {
                AllowedTypes = new Dictionary<string, DocumentTypeValidationRuleDto>()
            };
        }

        [Test]
        [TestCase(ActionType.ProofOfIdentity, DocumentCategory.ProofOfIdentity)]
        [TestCase(ActionType.ProofOfAddress, DocumentCategory.ProofOfAddress)]
        [TestCase(ActionType.ProofOfFunds, DocumentCategory.ProofOfFunds)]
        [TestCase(ActionType.Selfie, DocumentCategory.Selfie)]
        [TestCase(ActionType.W9Form, DocumentCategory.Taxation)]
        public void MapDocumentValidationRule_ShouldMapCorrectDocumentCategory_WithProvidedActionType(ActionType providedActionType, DocumentCategory expectedDocumentCategory)
        {
            // act
            var result = _mapper.MapDocumentValidationRule(providedActionType, _documentValidationRuleDto);
            
            // assert
            result.DocumentCategory.Should().Be(expectedDocumentCategory);
        }
        
        [Test]
        [TestCase(ActionType.Survey)]
        [TestCase(ActionType.Tin)]
        [TestCase(ActionType.TaxResidence)]
        public void MapDocumentValidationRule_ShouldThrowArgumentOutOfRangeException_WhenActionTypeIsNotSupported(ActionType providedActionType)
        {
            // act
            Action action = () =>
            {
                _mapper.MapDocumentValidationRule(providedActionType, _documentValidationRuleDto);
            };

            // assert
            var errorMessage = $"Unsupported action type. (Parameter 'actionType')\nActual value was {providedActionType}.";
            action.Should().Throw<ArgumentOutOfRangeException>().WithMessage(errorMessage);
        }
    }
}

