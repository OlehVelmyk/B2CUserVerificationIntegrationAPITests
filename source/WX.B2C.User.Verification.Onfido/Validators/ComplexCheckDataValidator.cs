using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Validators
{
    internal class ComplexCheckDataValidator : BaseCheckInputValidator<ComplexCheckInputData>
    {
        private readonly ComplexCheckConfiguration _configuration;

        public ComplexCheckDataValidator(ComplexCheckConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        protected override IEnumerable<string> Validate(CheckInputData inputData, out ComplexCheckInputData validatedData)
        {
            var missingData = new List<string>();

            var selfieXPath = GetSelfieXPath(_configuration.Configurations);

            validatedData = new ComplexCheckInputData
            {
                ApplicantId = inputData.ExternalProfileId,
                FullName = TryGetValue<FullNameDto>(XPathes.FullName),
                Selfie = TryGetValue<DocumentDto>(selfieXPath),
                BirthDate = TryGetValue<DateTime?>(XPathes.Birthdate),
                Address = TryGetValue<AddressDto>(XPathes.ResidenceAddress),
                IdentityDocument = TryGetValue<DocumentDto>(XPathes.ProofOfIdentityDocument)
            };

            return missingData;

            T TryGetValue<T>(string xPath) => IsRequired(xPath) ? inputData.TryGetValue<T>(xPath, missingData) : default;
            bool IsRequired(string xPath) => _configuration.CheckParameters.Any(parameter => parameter.XPath == xPath);

        }

        private static string GetSelfieXPath(IReadOnlyCollection<CheckProviderConfiguration> configurations)
        {
            var facialSimilarity = configurations
                                   .OfType<FacialSimilarityCheckConfiguration>()
                                   .Select(configuration => configuration.SelfieXPath);

            var faceDuplication = configurations
                                  .OfType<FaceDuplicationCheckConfiguration>()
                                  .Select(configuration => configuration.SelfieXPath);

            return facialSimilarity.Concat(faceDuplication).FirstOrDefault();
        }
    }
}
