using System;
using WX.B2C.User.Verification.Worker.Jobs.Models.Verification;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class SurveyCheckData : IJobData
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid SurveyId { get; set; }

        public SurveyCheckType Type { get; set; }

        public SurveyCheckStatus Status { get; set; }

        public SurveyCheckResult Result { get; set; }
    }
}