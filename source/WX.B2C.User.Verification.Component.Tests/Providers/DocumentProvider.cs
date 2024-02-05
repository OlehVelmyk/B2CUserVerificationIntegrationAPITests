using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Mappers;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;
using WX.B2C.User.Verification.Extensions;
using PublicApi = WX.B2C.User.Verification.Api.Public.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Providers
{
    internal class DocumentProvider
    {
        private readonly PublicApi.ActionType[] _documentActionsTypes = new[]
        {
            PublicApi.ActionType.Selfie,
            PublicApi.ActionType.W9Form,
            PublicApi.ActionType.ProofOfAddress,
            PublicApi.ActionType.ProofOfFunds,
            PublicApi.ActionType.ProofOfIdentity
        };
        private readonly PublicApiClientFactory _publicApiClientFactory;

        public DocumentProvider(PublicApiClientFactory publicApiClientFactory)
        {
            _publicApiClientFactory = publicApiClientFactory ?? throw new ArgumentNullException(nameof(publicApiClientFactory));
        }

        public async Task<DocumentCategory[]> GetAvailableCategoriesAsync(Guid userId)
        {
            var client = _publicApiClientFactory.Create(userId);

            var actions = await client.Actions.GetAsync();

            return actions.Where(action => _documentActionsTypes.Contains(action.ActionType))
                          .Select(action => ActionMapper.Map(action.ActionType))
                          .ToArray();
        }

        public async Task<FileToUpload> GetFileToUploadAsync(Guid userId, Seed seed)
        {
            var faker = FakerFactory.Create(seed);
            var fileFactory = new FileDataFactory(seed);
            var client = _publicApiClientFactory.Create(userId);

            var availableCategories = await GetAvailableCategoriesAsync(userId);
            var documentCategory = faker.PickRandomAndDump(availableCategories);
            var validationRules = await client.Validation.GetDocumentRulesAsync(documentCategory.To<PublicApi.DocumentCategory>());
            var validationRule = faker.PickRandom(validationRules.ValidationRules);

            var documentType = validationRule.DocumentType;
            var fileExtension = faker.PickRandomAndDump(validationRule.Extensions);
            var fileData = fileFactory.Create(documentType, fileExtension);
            return new FileToUpload(documentCategory, documentType, fileData);
        }

        public DocumentToSubmit GetDocumentToSubmit(DocumentCategory category, string type, int filesQuantity, string extension, Seed seed)
        {
            var fileFactory = new FileDataFactory(seed);
            var files = Enumerable.Range(0, filesQuantity)
                                  .Select(_ => fileFactory.Create(type, extension))
                                  .ToArray();

            return new DocumentToSubmit(category, type, files);
        }

        public async Task<DocumentToSubmit> GetDocumentToSubmitAsync(Guid userId, DocumentCategory category, Seed seed, int? filesQuantity = null)
        {
            var faker = FakerFactory.Create(seed);
            var fileFactory = new FileDataFactory(seed);
            var client = _publicApiClientFactory.Create(userId);

            var validationRules = await client.Validation.GetDocumentRulesAsync(category.To<PublicApi.DocumentCategory>());
            var validationRule = faker.PickRandom(validationRules.ValidationRules);
            var documentType = validationRule.DocumentType;
            filesQuantity = filesQuantity ?? GetFilesQuantity(validationRule, seed);

            return GetDocumentToSubmit(category, documentType, filesQuantity.Value, faker.PickRandom(validationRule.Extensions), seed);
        }

        public async Task<DocumentToSubmit> GetDocumentToSubmitAsync(Guid userId, Seed seed, int? filesQuantity = null)
        {
            var faker = FakerFactory.Create(seed);
            var availableCategories = await GetAvailableCategoriesAsync(userId);
            var documentCategory = faker.PickRandomAndDump(availableCategories);

            return await GetDocumentToSubmitAsync(userId, documentCategory, seed, filesQuantity);
        }

        public static int GetInvalidFilesQuantity(PublicApi.DocumentValidationRuleItemDto validationRule, Seed seed)
        {
            const int MaxFiles = 10;
            var faker = FakerFactory.Create(seed);

            if (validationRule.DocumentSide.HasValue)
                return validationRule.DocumentSide switch
                {
                    PublicApi.DocumentSide.Front => faker.Random.Number(2, MaxFiles),
                    PublicApi.DocumentSide.Back  => faker.Random.Number(2, MaxFiles),
                    PublicApi.DocumentSide.Both  => faker.Random.Number(0, 2),
                    _                            => throw new ArgumentOutOfRangeException(nameof(validationRule.DocumentSide))
                };

            if (validationRule.MinQuantity.HasValue && validationRule.MaxQuantity.HasValue)
            {
                return faker.PickRandom(new [] { faker.Random.Number(0, validationRule.MinQuantity.Value - 1),
                                                 faker.Random.Number(validationRule.MaxQuantity.Value + 1, MaxFiles)});
            }

            return 0;
        }

        private static int GetFilesQuantity(PublicApi.DocumentValidationRuleItemDto validationRule, Seed seed)
        {
            if (validationRule.DocumentSide.HasValue)
                return validationRule.DocumentSide switch
                {
                    PublicApi.DocumentSide.Front => 1,
                    PublicApi.DocumentSide.Back  => 1,
                    PublicApi.DocumentSide.Both  => 2,
                    _                            => throw new ArgumentOutOfRangeException(nameof(validationRule.DocumentSide))
                };

            if (validationRule.MinQuantity.HasValue && validationRule.MaxQuantity.HasValue)
            {
                var faker = FakerFactory.Create(seed);
                return faker.PickRandom(validationRule.MinQuantity.Value, validationRule.MaxQuantity.Value);
            }

            return 1;
        }
    }
}
