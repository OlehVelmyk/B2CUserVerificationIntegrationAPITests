using System;
using WX.B2C.User.Verification.Events.Dtos;
using WX.B2C.User.Verification.Events.Enums;

namespace WX.B2C.User.Verification.Events.EventArgs
{
    /// <summary>
    /// Predefined rejections reasons which could be set in <see cref="ApplicationStateChangedEventArgs.DecisionReasons"/>.
    /// </summary>
    public static class ApplicationRejectionReasons
    {
        public const string Fraud = nameof(Fraud);
        public const string UserLocked = nameof(UserLocked);
        public const string InstantIdClosing = nameof(InstantIdClosing);
        public const string DuplicateAccount = "Duplicate account";
        public const string VerificationPolicyChanged = nameof(VerificationPolicyChanged);
    }

    public class ApplicationStateChangedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public ApplicationState PreviousState { get; set; }

        public ApplicationState NewState { get; set; }

        /// <summary>
        /// Collection of reasons why application is moved to corresponding state.
        /// Can contain predefined reason from <see cref="ApplicationRejectionReasons"/>.
        /// </summary>
        public string[] DecisionReasons { get; set; }

        public InitiationDto Initiation { get; set; }

        public static ApplicationStateChangedEventArgs Create(Guid userId,
                                                              Guid applicationId,
                                                              ApplicationState previousState,
                                                              ApplicationState newState,
                                                              string[] decisionReasons,
                                                              InitiationDto initiationDto) =>
            new ApplicationStateChangedEventArgs
            {
                UserId = userId,
                ApplicationId = applicationId,
                Initiation = initiationDto,
                NewState = newState,
                DecisionReasons = decisionReasons,
                PreviousState = previousState,
            };
    }
}
