using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Validators
{
    internal sealed class FacialSimilarityCheckDataValidator : BaseCheckInputValidator<FacialSimilarityCheckInputData>
    {
        private readonly FacialSimilarityCheckConfiguration _configuration;

        public FacialSimilarityCheckDataValidator(FacialSimilarityCheckConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override IEnumerable<string> Validate(CheckInputData inputData, out FacialSimilarityCheckInputData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new FacialSimilarityCheckInputData
            {
                ApplicantId = inputData.ExternalProfileId,
                FullName = inputData.TryGetValue<FullNameDto>(XPathes.FullName, missingData),
                IdentityDocument = inputData.TryGetValue<DocumentDto>(XPathes.ProofOfIdentityDocument, missingData),
                Selfie = inputData.TryGetValue<DocumentDto>(_configuration.SelfieXPath, missingData)
            };

            return missingData;
        }
    }
}