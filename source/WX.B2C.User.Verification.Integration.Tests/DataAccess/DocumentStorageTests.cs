using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Extensions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    internal class DocumentStorageTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private IDocumentStorage _sut;
        private IDocumentRepository _documentRepository;
        private IFileRepository _fileRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = Resolve<IDocumentStorage>();
            _documentRepository = Resolve<IDocumentRepository>();
            _fileRepository = Resolve<IFileRepository>();
            Arb.Register<DocumentTypeArbitrary>();
            Arb.Register<DocumentArbitrary>();
            Arb.Register<FileArbitrary>();
            Arb.Register<ArrayArbitrary<DocumentSpecimen>>();
            Arb.Register<ArrayArbitrary<DocumentCategory>>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task GetAsync_ShouldFindDocument(DocumentSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            await specimen.Files.Foreach(documentFile => _fileRepository.SaveAsync(documentFile.File));
            var document = new DocumentBuilder().From(specimen).Build();
            await _documentRepository.SaveAsync(document);

            // Act
            var actual = await _sut.GetAsync(document.Id);

            // Assert
            actual.Should().BeEquivalentTo(document, _assertionSettings);
        }

        [Theory]
        public async Task GetAsync_ShouldFindDocuments(DocumentSpecimen[] specimens, Guid userId, DocumentCategory documentCategory)
        {
            // Given
            await specimens.Foreach(async specimen =>
            {
                _users.Add(userId);
                (specimen.UserId, specimen.Category) = (userId, documentCategory);
                await specimen.Files.Foreach(documentFile => _fileRepository.SaveAsync(documentFile.File));
                var document = new DocumentBuilder().From(specimen).Build();
                await _documentRepository.SaveAsync(document);
            });

            // Act
            var result = await _sut.FindAsync(userId, documentCategory);

            // Assert
            result.Should().NotBeNullOrEmpty().And.HaveSameCount(specimens);
            result.Foreach(actual =>
            {
                actual.Should().NotBeNull();
                var specimen = specimens.First(specimen => specimen.Id == actual.Id);
                var document = new DocumentBuilder().From(specimen).Build();
                actual.Should().BeEquivalentTo(document, _assertionSettings);
            });
        }

        [Theory]
        public async Task GetAsync_ShouldFindDocumentsWithoutCategory(DocumentSpecimen[] specimens, Guid userId, DocumentCategory? documentCategory)
        {
            // Given
            await specimens.Foreach(async specimen =>
            {
                _users.Add(userId);
                specimen.UserId = userId;
                await specimen.Files.Foreach(documentFile => _fileRepository.SaveAsync(documentFile.File));
                var document = new DocumentBuilder().From(specimen).Build();
                await _documentRepository.SaveAsync(document);
            });

            // Act
            var result = await _sut.FindAsync(userId, null);

            // Assert
            result.Should().NotBeNullOrEmpty().And.HaveSameCount(specimens);
            result.Foreach(actual =>
            {
                actual.Should().NotBeNull();
                var specimen = specimens.First(specimen => specimen.Id == actual.Id);
                var document = new DocumentBuilder().From(specimen).Build();
                actual.Should().BeEquivalentTo(document, _assertionSettings);
            });
        }

        [Theory]
        public async Task FindSubmittedDocumentsAsync_ShouldFindDocumentsByUserId(DocumentSpecimen[] specimens, Guid userId)
        {
            // Given
            await specimens.Foreach(async specimen =>
            {
                _users.Add(userId);
                (specimen.UserId, specimen.Status) = (userId, DocumentStatus.Submitted);
                await specimen.Files.Foreach(documentFile => _fileRepository.SaveAsync(documentFile.File));
                var document = new DocumentBuilder().From(specimen).Build();
                await _documentRepository.SaveAsync(document);
            });

            // Act
            var result = await _sut.FindSubmittedDocumentsAsync(userId);

            // Assert
            result.Should().NotBeNullOrEmpty().And.HaveSameCount(specimens);
            result.Foreach(actual =>
            {
                actual.Should().NotBeNull();
                var specimen = specimens.First(specimen => specimen.Id == actual.Id);
                var document = new DocumentBuilder().From(specimen).Build();
                actual.Should().BeEquivalentTo(document, _assertionSettings);
            });
        }

        [Theory]
        public async Task FindSubmittedDocumentsAsync_ShouldFindDocumentsByUserIdAndDocumentCategories(DocumentSpecimen[] specimens,
                                                                                                       Guid userId,
                                                                                                       DocumentCategory[] categories)
        {
            // Given
            int count = 0;
            await specimens.Foreach(async specimen =>
            {
                _users.Add(userId);
                (specimen.UserId, specimen.Status) = (userId, DocumentStatus.Submitted);
                specimen.Category = categories[count++ % categories.Length];
                await specimen.Files.Foreach(documentFile => _fileRepository.SaveAsync(documentFile.File));
                var document = new DocumentBuilder().From(specimen).Build();
                await _documentRepository.SaveAsync(document);
            });

            // Act
            var result = await _sut.FindSubmittedDocumentsAsync(userId, categories);

            // Assert
            result.Should().NotBeNullOrEmpty().And.HaveSameCount(specimens);
            result.Foreach(actual =>
            {
                actual.Should().NotBeNull();
                var specimen = specimens.First(specimen => specimen.Id == actual.Id);
                var document = new DocumentBuilder().From(specimen).Build();
                actual.Should().BeEquivalentTo(document, _assertionSettings);
            });
        }
        
        private readonly Func<EquivalencyAssertionOptions<DocumentDto>, EquivalencyAssertionOptions<DocumentDto>>
            _assertionSettings = options =>
            {
                return options.Using<DateTime>(context => context.Subject.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds()))
                              .When(info => info.Path.Contains(nameof(AuditableEntity.CreatedAt)));
            };
    }
}
