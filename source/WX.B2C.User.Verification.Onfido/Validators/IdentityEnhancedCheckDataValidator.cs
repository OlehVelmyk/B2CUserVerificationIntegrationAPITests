using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Validators
{
    internal class IdentityEnhancedCheckDataValidator : BaseCheckInputValidator<IdentityEnhancedCheckInputData>
    {
        public IdentityEnhancedCheckDataValidator(IdentityEnhancedCheckConfiguration configuration)
            : base(configuration)
        {
        }

        protected override IEnumerable<string> Validate(CheckInputData inputData, out IdentityEnhancedCheckInputData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new IdentityEnhancedCheckInputData
            {
                ApplicantId = inputData.ExternalProfileId,
                FullName = inputData.TryGetValue<FullNameDto>(XPathes.FullName, missingData),
                BirthDate = inputData.TryGetValue<DateTime>(XPathes.Birthdate, missingData),
                Address = inputData.TryGetValue<AddressDto>(XPathes.ResidenceAddress, missingData)
            };

            return missingData;
        }
    }
}