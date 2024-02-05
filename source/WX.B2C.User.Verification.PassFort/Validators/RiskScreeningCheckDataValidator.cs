using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.PassFort.Configuration;
using WX.B2C.User.Verification.PassFort.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.PassFort.Validators
{
    internal sealed class RiskScreeningCheckDataValidator : BaseCheckInputValidator<RiskScreeningCheckData>
    {
        public RiskScreeningCheckDataValidator(RiskScreeningCheckConfiguration configuration)
            : base(configuration) { }

        protected override IEnumerable<string> Validate(CheckInputData inputData, out RiskScreeningCheckData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new RiskScreeningCheckData
            {
                ProfileId = inputData.ExternalProfileId,
                FullName = inputData.TryGetValue<FullNameDto>(XPathes.FullName, missingData),
                Nationality = inputData.TryGetValue<string>(XPathes.VerifiedNationality, missingData),
                BirthDate = inputData.TryGetValue<DateTime>(XPathes.Birthdate, missingData),
                ResidenceAddress = inputData.TryGetValue<AddressDto>(XPathes.ResidenceAddress, missingData)
            };

            if (string.IsNullOrEmpty(validatedData.ProfileId))
                missingData.Add(nameof(validatedData.ProfileId));

            return missingData;
        }
    }
}
