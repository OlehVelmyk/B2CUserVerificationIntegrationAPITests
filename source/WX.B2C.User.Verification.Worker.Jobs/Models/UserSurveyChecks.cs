using System;
using WX.B2C.User.Verification.Worker.Jobs.Models.Verification;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class UserSurveyChecks : IJobData
    {
        public Guid UserId { get; set; }

        public SurveyCheck[] Checks { get; set; }
    }

    internal class SurveyCheck
    {
        public SurveyCheckType Type { get; set; }

        public SurveyCheckStatus Status { get; set; }

        public SurveyCheckResult Result { get; set; }
    }
}