using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class DuplicateScreeningCheckDataValidator : BaseCheckInputValidator<DuplicateScreeningInputData>
    {
        public DuplicateScreeningCheckDataValidator(DuplicateScreeningCheckConfiguration configuration)
            : base(configuration) { }

        protected override IEnumerable<string> Validate(CheckInputData checkData, out DuplicateScreeningInputData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new DuplicateScreeningInputData
            {
                UserId = checkData.UserId,
                FullName = checkData.TryGetValue<FullNameDto>(XPathes.FullName, missingData),
                BirthDate = checkData.TryGetValue<DateTime>(XPathes.Birthdate, missingData)
            };

            return missingData;
        }
    }
}