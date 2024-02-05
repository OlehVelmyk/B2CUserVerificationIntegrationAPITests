using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class ApplicationStateChangedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public ApplicationState PreviousState { get; set; }

        public ApplicationState NewState { get; set; }

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