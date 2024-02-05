using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal class SurveyAnswersCheckConfiguration : CheckProviderConfiguration
    {
        public Guid TemplateId { get; set; }

        public Dictionary<string, string[]> RiskAnswers { get; set; }

        public override IEnumerable<CheckInputParameter> CheckParameters => new[]
        {
            CheckInputParameter.Required(new SurveyXPath(TemplateId))
        };
    }
}