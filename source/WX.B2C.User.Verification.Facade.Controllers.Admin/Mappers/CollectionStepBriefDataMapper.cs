using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using CoreDtos = WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface ICollectionStepBriefDataMapper
    {
        public Task<IDictionary<string, CollectionStepBriefDataDto>> MapAsync(IReadOnlyDictionary<string, object> collectionSteps);
    }

    internal class CollectionStepBriefDataMapper : ICollectionStepBriefDataMapper
    {
        private readonly ISurveyTemplatesProvider _surveyTemplatesProvider;
        private readonly ICollectionStepNameMapper _collectionStepNameMapper;
        private readonly IXPathParser _xPathParser;

        public CollectionStepBriefDataMapper(ISurveyTemplatesProvider surveyTemplatesProvider,
                                             ICollectionStepNameMapper collectionStepNameMapper,
                                             IXPathParser xPathParser)
        {
            _surveyTemplatesProvider = surveyTemplatesProvider ?? throw new ArgumentNullException(nameof(surveyTemplatesProvider));
            _collectionStepNameMapper = collectionStepNameMapper ?? throw new ArgumentNullException(nameof(collectionStepNameMapper));
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
        }

        public async Task<IDictionary<string, CollectionStepBriefDataDto>> MapAsync(IReadOnlyDictionary<string, object> profileData)
        {
            if (profileData == null)
                throw new ArgumentNullException(nameof(profileData));

            var xpathes = profileData.Select(pair => pair.Key).ToArray();
            var (propertyXpathes, documentsXpathes, surveyXpathes) = AggregateByType(xpathes);

            var mappedDocuments = documentsXpathes.Select(xpath => MapDocument(xpath, profileData.Get<CoreDtos.DocumentDto>(xpath.XPath)));
            var mappedSurveys = await MapSurveysAsync(surveyXpathes.ToArray());
            var mappedProperties = propertyXpathes.Select(propertyXpath => MapProperty(propertyXpath, profileData.Get<object>(propertyXpath.XPath)));

            return mappedDocuments.Concat(mappedSurveys)
                                  .Concat(mappedProperties)
                                  .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private KeyValuePair<string, CollectionStepBriefDataDto> MapProperty(PropertyXPathDetails xpath, object value)
        {
            var name = _collectionStepNameMapper.Map(xpath);
            var mappedValue = xpath.Property switch
            {
                VerificationProperty.IpAddress => MapIpAddress(value),
                VerificationProperty.Nationality => MapNationality(value),
                VerificationProperty.IdDocumentNumber => MapIdDocNumber(value),
                VerificationProperty.TaxResidence => MapTaxResidences(value),
                VerificationProperty.Tin => MapTin(value),
                PersonalProperty.Birthdate => MapBirthdate(value),
                PersonalProperty.FullName => MapFullName(value),
                PersonalProperty.ResidenceAddress => MapResidenceAddress(value),
                _ => value.ToString()
            };

            return CreateKeyValuePair(xpath.XPath,
                                      new CollectionStepBriefDataDto
                                      {
                                          Name = name,
                                          Value = mappedValue
                                      });
        }


        private async Task<IEnumerable<KeyValuePair<string, CollectionStepBriefDataDto>>> MapSurveysAsync(SurveyXPathDetails[] xPathDetails)
        {
            if (xPathDetails.IsNullOrEmpty())
                return Array.Empty<KeyValuePair<string, CollectionStepBriefDataDto>>();

            var templateIds = xPathDetails.Select(xpath => xpath.SurveyId).ToArray();
            var templates = await _surveyTemplatesProvider.GetAsync(templateIds);
            return templates.Select(
                template => CreateKeyValuePair(xPathDetails.First(xpath => xpath.SurveyId == template.TemplateId).XPath,
                                               new CollectionStepBriefDataDto
                                               {
                                                   Name = "Survey",
                                                   Value = template.Name
                                               }));
        }

        private KeyValuePair<string, CollectionStepBriefDataDto> MapDocument(DocumentsXPathDetails documentsXPathDetails, CoreDtos.DocumentDto documentDto)
        {
            return CreateKeyValuePair(
                documentsXPathDetails.XPath,
                new CollectionStepBriefDataDto
                {
                    Name = _collectionStepNameMapper.Map(documentsXPathDetails),
                    Value = documentDto.Type.ToString()
                });
        }


        private (IEnumerable<PropertyXPathDetails>, IEnumerable<DocumentsXPathDetails>, IEnumerable<SurveyXPathDetails>) AggregateByType(string[] xpathes)
        {
            var documents = xpathes.Select(_xPathParser.Parse)
                                   .Select(xpathDetails => xpathDetails as DocumentsXPathDetails)
                                   .Where(xpathDetails => xpathDetails is not null);

            var properties = xpathes.Select(_xPathParser.Parse)
                                    .Select(xpathDetails => xpathDetails as PropertyXPathDetails)
                                    .Where(xpathDetails => xpathDetails is not null);

            var surveys = xpathes.Select(_xPathParser.Parse)
                                 .Select(xpathDetails => xpathDetails as SurveyXPathDetails)
                                 .Where(xpathDetails => xpathDetails is not null);

            return (properties, documents, surveys);
        }

        private static string MapNationality(object value)
        {
            return (string)value;
        }

        private static string MapBirthdate(object value)
        {
            var dateTime = (DateTime)value;
            return dateTime.ToString("d");
        }

        private static string MapResidenceAddress(object value)
        {
            var addressDto = (CoreDtos.Profile.AddressDto)value;
            return $"{addressDto.Country}, {addressDto.Line1}, {AddIfNonNull(addressDto.Line2)}{addressDto.Line2}{addressDto.City}, " +
                   $"{AddIfNonNull(addressDto.State)}{addressDto.ZipCode}";

            string AddIfNonNull(string s) => s is null ? "" : $"{s}, ";
        }

        private static string MapFullName(object value)
        {
            var fullName = (CoreDtos.FullNameDto)value;
            return $"{fullName.FirstName} {fullName.LastName}";
        }

        private static string MapTin(object value)
        {
            var tinDto = (CoreDtos.Profile.TinDto)value;
            return $"{tinDto.Number} ({tinDto.Type})";
        }

        private static string MapTaxResidences(object value)
        {
            var taxResidences = (string[])value;
            return string.Join(", ", taxResidences);
        }

        private static string MapIdDocNumber(object value)
        {
            var idDocNumberDto = (CoreDtos.Profile.IdDocumentNumberDto)value;
            return $"{idDocNumberDto.Number} ({idDocNumberDto.Type})";
        }

        private static string MapIpAddress(object value)
        {
            var ipAddress = (string)value;
            return ipAddress;
        }

        private static KeyValuePair<string, CollectionStepBriefDataDto> CreateKeyValuePair(string key, CollectionStepBriefDataDto value) =>
            new(key, value);
    }
}
