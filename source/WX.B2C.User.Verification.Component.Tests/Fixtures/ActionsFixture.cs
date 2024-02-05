using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class ActionsFixture
    {
        private static readonly IReadOnlyDictionary<Type, (ActionType[] actions, (string id, string tag)[] surveys)> _expactations = CreateExpactations();

        public static (ActionType[] actions, (string id, string tag)[] surveys) GetExpectedActions(UserInfo userInfo) =>
            _expactations[userInfo.GetType()];

        private static IReadOnlyDictionary<Type, (ActionType[] actions, (string id, string tag)[] surveys)> CreateExpactations() =>
            new Dictionary<Type, (ActionType[] actions, (string id, string tag)[] surveys)>()
            {
                {
                    typeof(GbUserInfo),
                    (
                        new[] { ActionType.TaxResidence, ActionType.ProofOfIdentity, ActionType.Selfie, ActionType.Survey },
                        new[] { (Surveys.UkOnboardingSurvey, Surveys.UkOnboardingTag) }
                    )
                },
                {
                    typeof(UsUserInfo),
                    (
                        new[] { ActionType.TaxResidence, ActionType.ProofOfIdentity, ActionType.Selfie, ActionType.Survey, ActionType.Tin },
                        new[] { (Surveys.UsCddSurvey, Surveys.UsCddTag) }
                    )
                },
                {
                    typeof(EeaUserInfo),
                    (
                        new[] { ActionType.TaxResidence, ActionType.ProofOfIdentity, ActionType.Selfie, ActionType.Survey },
                        new[] { (Surveys.EeaOnboardingSurvey, Surveys.EeaOnboardingTag) }
                    )
                },
                {
                    typeof(ApacUserInfo),
                    (
                        new[] { ActionType.TaxResidence, ActionType.ProofOfIdentity, ActionType.Selfie },
                        new (string, string)[0]
                    )
                },
                {
                    typeof(RoWUserInfo),
                    (
                        new[] { ActionType.TaxResidence, ActionType.ProofOfIdentity, ActionType.Selfie },
                        new (string, string)[0]
                    )
                },
                {
                    typeof(GlobalUserInfo),
                    (
                        new[] { ActionType.TaxResidence, ActionType.ProofOfIdentity, ActionType.Selfie, ActionType.ProofOfAddress },
                        new (string, string)[0]
                    )
                },
                {
                    typeof(RuUserInfo),
                    (
                        new[] { ActionType.TaxResidence, ActionType.ProofOfIdentity, ActionType.Selfie },
                        new (string, string)[0]
                    )
                }
            };
    }
}
