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
    internal sealed class FaceDuplicationCheckDataValidator : BaseCheckInputValidator<FaceDuplicationCheckInputData>
    {
        private readonly FaceDuplicationCheckConfiguration _configuration;

        public FaceDuplicationCheckDataValidator(FaceDuplicationCheckConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override IEnumerable<string> Validate(CheckInputData inputData, out FaceDuplicationCheckInputData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new FaceDuplicationCheckInputData
            {
                ApplicantId = inputData.ExternalProfileId,
                FullName = inputData.TryGetValue<FullNameDto>(XPathes.FullName, missingData),
                Selfie = inputData.TryGetValue<DocumentDto>(_configuration.SelfieXPath, missingData)
            };

            return missingData;
        }
    }
}