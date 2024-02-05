using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Facade.Controllers.Public.Services;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Unit.Tests.Mappers
{
    [TestFixture]
    public class DocumentCategoryMapperTests
    {
        private IDocumentCategoryMapper _documentCategoryMapper;
        private IActionTypeMapper _actionTypeMapper;
        
        [SetUp]
        public void Setup()
        {
            _documentCategoryMapper = new DocumentCategoryMapper();
            _actionTypeMapper = new ActionTypeMapper();
        }

        [Test]
        [TestCase(DocumentCategory.ProofOfIdentity, "any")]
        [TestCase(DocumentCategory.ProofOfAddress, "any")]
        [TestCase(DocumentCategory.ProofOfFunds, "any")]
        [TestCase(DocumentCategory.Selfie, "any")]
        [TestCase(DocumentCategory.Taxation, nameof(TaxationDocumentType.W9Form))]
        public void Map_ShouldReflectActionTypeMapper_WhenMapDocumentCategory(DocumentCategory providedDocumentCategory, string providedDocumentType)
        {
            // act
            var actionType = _actionTypeMapper.Map(providedDocumentCategory, providedDocumentType);
            var documentCategory = _documentCategoryMapper.Map(actionType.Value);
            
            // assert
            documentCategory.Should().Be(providedDocumentCategory);
        }
    }
}

