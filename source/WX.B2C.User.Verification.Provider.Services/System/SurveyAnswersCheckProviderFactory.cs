using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class SurveyAnswersCheckProviderFactory : BaseCheckProviderFactory<SurveyAnswersCheckConfiguration>
    {
        private readonly IUserSurveyProvider _userSurveyProvider;

        public SurveyAnswersCheckProviderFactory(IUserSurveyProvider userSurveyProvider)
        {
            _userSurveyProvider = userSurveyProvider ?? throw new ArgumentNullException(nameof(userSurveyProvider));
        }

        protected override CheckProvider Create(SurveyAnswersCheckConfiguration configuration)
        {
            var checkDataValidator = new SurveyAnswersCheckDataValidator(configuration);
            var checkRunner = new SurveyAnswersCheckRunner(configuration, _userSurveyProvider);
            return CheckProvider.Create(checkDataValidator, checkRunner);
        }
    }
}