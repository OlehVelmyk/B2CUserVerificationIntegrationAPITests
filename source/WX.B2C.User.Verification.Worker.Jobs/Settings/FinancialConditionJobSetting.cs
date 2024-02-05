using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class FinancialConditionJobSetting : UserBatchJobSettings
    {
        public Guid[] SurveyIds { get; set; }

        public Guid TaskVariantId { get; set; }
    }
}