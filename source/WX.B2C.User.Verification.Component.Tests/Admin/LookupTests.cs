using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    internal class LookupTests : BaseComponentTest
    {
        private AdministratorFactory _administratorFactory;
        private AdminApiClientFactory _adminApiClientFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _administratorFactory = Resolve<AdministratorFactory>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
        }

        /// <summary>
        /// Given admin
        /// When admin request survey templates info
        /// Then he receive it
        /// And it contains name and template id
        /// </summary>
        [Theory]
        public async Task ShouldGetSurveyTemplatesInfo()
        {
            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            
            // Act
            var surveys = await client.Lookup.GetSurveyTemplatesAsync();

            // Assert
            surveys.Should().NotBeEmpty();
            foreach (var template in surveys)
            {
                template.Name.Should().NotBeEmpty();
                template.TemplateId.Should().NotBeEmpty(); 
            }
        }

        /// <summary>
        /// Given admin
        /// When admin request document categories info
        /// Then he receive info about every document category
        /// And it contains name, description and types info 
        /// </summary>
        [Theory]
        public async Task ShouldGetDocumentCategoriesInfo()
        {
            // Arrange
            var expectedDocumentCategoriesInfo = DocumentCategoryProvider.GetAll();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            var documentCategoriesLookup = await client.Lookup.GetDocumentCategoriesAsync();

            // Assert
            documentCategoriesLookup.Should().NotBeEmpty();
            documentCategoriesLookup.Select(dc => dc.Name).Should().OnlyHaveUniqueItems();
            documentCategoriesLookup.Should().HaveSameCount(expectedDocumentCategoriesInfo);
            foreach (var documentCategoryInfo in documentCategoriesLookup)
            {
                documentCategoryInfo.Description.Should().NotBeEmpty();
                documentCategoryInfo.Types.Should().NotBeEmpty();

                var documentCategory = documentCategoryInfo.Name.To<Models.Enums.DocumentCategory>();
                expectedDocumentCategoriesInfo.TryGetValue(documentCategory, out var expectedDocumentTypes);
                documentCategoryInfo.Types.Select(typeLookup => typeLookup.Name).Should().BeEquivalentTo(expectedDocumentTypes);
                foreach (var type in documentCategoryInfo.Types)
                    type.Description.Should().NotBeEmpty();
            }
        }
    }
}
