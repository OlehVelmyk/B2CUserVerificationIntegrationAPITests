using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Optional;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services.RequiredData
{
    /// <summary>
    /// The main class to read required data from DB.
    /// Has state therefore must be user once per one user.
    /// All performance improvements logic connected to reading data must be encapsulated here.
    /// </summary>
    public class ProfileDataProvider : IProfileDataProvider
    {
        private readonly IXPathParser _xPathParser;
        private readonly IDetailsReader<VerificationDetailsDto> _verificationDetailsReader;
        private readonly IDetailsReader<PersonalDetailsDto> _personalDetailsReader;

        private readonly AsyncLazy<PersonalDetailsDto> _lazyPersonalDetails;
        private readonly AsyncLazy<VerificationDetailsDto> _lazyVerificationDetails;
        private readonly AsyncLazy<DocumentDto[]> _lazySubmittedDocuments;
        private readonly AsyncLazy<CollectionStepDto[]> _lazyCollectionSteps;

        internal ProfileDataProvider(Guid userId,
                                     ICollectionStepStorage collectionStepStorage,
                                     IProfileStorage profileStorage,
                                     IDocumentStorage documentStorage,
                                     IXPathParser xPathParser)
        {
            _lazyPersonalDetails = new AsyncLazy<PersonalDetailsDto>(() => profileStorage.FindPersonalDetailsAsync(userId));
            _lazyVerificationDetails = new AsyncLazy<VerificationDetailsDto>(() => profileStorage.FindVerificationDetailsAsync(userId));
            _lazySubmittedDocuments = new AsyncLazy<DocumentDto[]>(() => documentStorage.FindSubmittedDocumentsAsync(userId));
            _lazyCollectionSteps = new AsyncLazy<CollectionStepDto[]>(() => collectionStepStorage.FindRequestedAsync(userId));
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
            _verificationDetailsReader = new VerificationDetailsReader();
            _personalDetailsReader = new PersonalDetailsReader();
        }

        public async Task<IProfileDataCollection> ReadNotRequestedAsync(IEnumerable<string> xPathes)
        {
            var requestedXpath = await GetRequestedXPathesAsync();
            var xPathesToRead = xPathes.Distinct().Except(requestedXpath).ToArray();
            return await ReadAsync(xPathesToRead);
        }

        public async Task<IProfileDataCollection> ReadAsync(IEnumerable<string> xPathes)
        {
            xPathes = xPathes?.Distinct() ?? throw new ArgumentNullException(nameof(xPathes));

            var details = xPathes.Select(_xPathParser.Parse);
            var dataValues = await details.Foreach(xPathDetails =>
            {
                return xPathDetails switch
                {
                    DocumentsXPathDetails documentsXPathDetails => ReadDataFromDocumentsAsync(documentsXPathDetails),
                    PropertyXPathDetails { Source: PropertySource.Personal } personalDetails => ReadDataFromPersonalDetailsAsync(personalDetails),
                    PropertyXPathDetails { Source: PropertySource.Verification } verificationDetails => ReadDataFromVerificationDetailsAsync(verificationDetails),
                    SurveyXPathDetails surveyXPathDetails => ParseSurveyTemplateDataAsync(surveyXPathDetails),
                    _ => throw new ArgumentOutOfRangeException(nameof(xPathDetails), xPathDetails.GetType(), "Not supported XPath type.")
                };
            });

            var dataCollection = dataValues
                                 .Where(value => value.HasValue)
                                 .ToDictionary(x => x.XPath, x => x.Value);

            return new ProfileDataCollection(dataCollection);
        }

        private async Task<IEnumerable<string>> GetRequestedXPathesAsync()
        {
            return (await _lazyCollectionSteps).Select(dto => dto.XPath).Distinct();
        }

        private static Task<DataValue> ParseSurveyTemplateDataAsync(SurveyXPathDetails surveyXPathDetails)
        {
            var result = new DataValue(surveyXPathDetails.XPath, surveyXPathDetails.SurveyId);
            return Task.FromResult(result);
        }

        private async Task<DataValue> ReadDataFromDocumentsAsync(DocumentsXPathDetails documentsXPathDetails)
        {
            object value = null;
            var source = await _lazySubmittedDocuments;
            if (source != null)
            {
                value = documentsXPathDetails.Type.SomeNotNull().Match(
                    type => source.FirstOrDefault(dto => dto.Type == type),
                    () => source.FirstOrDefault(dto => dto.Category == documentsXPathDetails.Category));
            }

            return new DataValue(documentsXPathDetails.XPath, value);
        }

        private async Task<DataValue> ReadDataFromVerificationDetailsAsync(PropertyXPathDetails verificationDetailsDto)
        {
            object value = null;
            var source = await _lazyVerificationDetails;
            if (source != null)
                value = _verificationDetailsReader.Read(source, verificationDetailsDto.XPath);

            return new DataValue(verificationDetailsDto.XPath, value);
        }

        private async Task<DataValue> ReadDataFromPersonalDetailsAsync(PropertyXPathDetails personalDetailsDto)
        {
            object value = null;
            var source = await _lazyPersonalDetails;
            if (source != null)
                value = _personalDetailsReader.Read(source, personalDetailsDto.XPath);

            return new DataValue(personalDetailsDto.XPath, value);
        }

        private class ProfileDataCollection : Dictionary<string, object>, IProfileDataCollection
        {
            public ProfileDataCollection(IDictionary<string, object> dataCollection)
                : base(dataCollection)
            {
            }

            public bool HasValue(string xPath) =>
                ContainsKey(xPath);

            public object ValueOrNull(string xPath) =>
                TryGetValue(xPath, out var value) ? value : null;
        }

        private class DataValue
        {
            public DataValue(string xPath, object value)
            {
                XPath = xPath;
                Value = value;
                HasValue = value != null;
            }

            public bool HasValue { get; }

            public string XPath { get; }

            public object Value { get; }
        }
    }
}