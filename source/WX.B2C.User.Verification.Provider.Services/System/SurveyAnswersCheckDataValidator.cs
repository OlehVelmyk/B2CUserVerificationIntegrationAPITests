using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class SurveyAnswersCheckDataValidator : BaseCheckInputValidator<SurveyAnswersCheckInputData>
    {
        private readonly SurveyAnswersCheckConfiguration _configuration;

        public SurveyAnswersCheckDataValidator(SurveyAnswersCheckConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override IEnumerable<string> Validate(CheckInputData checkData, out SurveyAnswersCheckInputData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new SurveyAnswersCheckInputData
            {
                UserId = checkData.UserId,
                TemplateId = checkData.TryGetValue<Guid>(new SurveyXPath(_configuration.TemplateId), missingData)
            };

            return missingData;
        }
    }
}
