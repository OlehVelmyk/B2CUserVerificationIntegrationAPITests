using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Validators
{
    internal sealed class IdentityCheckDataValidator : BaseCheckInputValidator<IdentityDocumentsCheckInputData>
    {
        public IdentityCheckDataValidator(IdentityDocumentsCheckConfiguration configuration)
            : base(configuration)
        {
        }

        protected override IEnumerable<string> Validate(CheckInputData inputData, out IdentityDocumentsCheckInputData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new IdentityDocumentsCheckInputData
            {
                ApplicantId = inputData.ExternalProfileId,
                FullName = inputData.TryGetValue<FullNameDto>(XPathes.FullName, missingData),
                BirthDate = inputData.TryGetValue<DateTime>(XPathes.Birthdate, missingData),
                IdentityDocument = inputData.TryGetValue<DocumentDto>(XPathes.ProofOfIdentityDocument, missingData)
            };

            return missingData;
        }
    }
}