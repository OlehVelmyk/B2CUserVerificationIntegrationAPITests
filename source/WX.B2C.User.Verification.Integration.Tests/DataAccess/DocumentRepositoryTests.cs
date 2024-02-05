using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Extensions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    internal class DocumentRepositoryTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private IDocumentRepository _sut;
        private IFileRepository _fileRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = Resolve<IDocumentRepository>();
            _fileRepository = Resolve<IFileRepository>();
            Arb.Register<DocumentTypeArbitrary>();
            Arb.Register<DocumentArbitrary>();
            Arb.Register<FileArbitrary>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task SaveAsync_ShouldSaveDocument(DocumentSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            await specimen.Files.Foreach(documentFile => _fileRepository.SaveAsync(documentFile.File));
            var document = new DocumentBuilder().From(specimen).Build();

            // Act 
            await _sut.SaveAsync(document);

            // Assert
            var actual = DbFixture.DbContext.Find<Document>(document.Id);
            await DbFixture.DbContext.Entry(actual).Collection(x => x.Files).LoadAsync();
            actual.Should().NotBeNull();
            actual.Files.Should().HaveSameCount(specimen.Files);
        }

        [Theory]
        public async Task GetAsync_ShouldFindDocument(DocumentSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            await specimen.Files.Foreach(documentFile => _fileRepository.SaveAsync(documentFile.File));
            var document = new DocumentBuilder().From(specimen).Build();
            await _sut.SaveAsync(document);

            // Act
            var actual = await _sut.GetAsync(document.Id);

            // Assert
            actual.Should().BeEquivalentTo(document, _assertionSettings);
        }

        [Theory]
        public async Task SaveAsync_ShouldUpdateDocument(DocumentSpecimen oldSpecimen,
                                                         DocumentSpecimen newSpecimen)
        {
            // Given
            _users.Add(oldSpecimen.UserId);
            await oldSpecimen.Files.Foreach(documentFile => _fileRepository.SaveAsync(documentFile.File));
            var old = new DocumentBuilder().From(oldSpecimen).Build();
            await _sut.SaveAsync(old);

            var saved = await _sut.GetAsync(old.Id);
            saved.Should().BeEquivalentTo(old, _assertionSettings);

            // Arrange
            (newSpecimen.Id, newSpecimen.UserId) = (old.Id, old.UserId);
            newSpecimen.Files = oldSpecimen.Files;
            var expected = new DocumentBuilder().From(newSpecimen).Build();

            // Act
            await _sut.SaveAsync(expected);

            // Assert
            var actual = await _sut.GetAsync(expected.Id);
            actual.Should().BeEquivalentTo(expected, _assertionSettings);
        }

        private readonly Func<EquivalencyAssertionOptions<DocumentDto>, EquivalencyAssertionOptions<DocumentDto>>
            _assertionSettings = options =>
            {
                return options.Using<DateTime>(context => context.Subject.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds()))
                              .When(info => info.Path.Contains(nameof(AuditableEntity.CreatedAt)));
            };
    }
}
