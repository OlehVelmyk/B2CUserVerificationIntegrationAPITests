using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal class SurveyAnswersCheckInputData
    {
        public Guid UserId { get; set; }

        public Guid TemplateId { get; set; }
    }

    internal class SurveyAnswersCheckOutputData : CheckOutputData
    {
        public Dictionary<string, string[]> Matches { get; set; }
    }
}