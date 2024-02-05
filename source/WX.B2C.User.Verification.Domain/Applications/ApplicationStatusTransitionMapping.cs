using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Domain.Extensions
{
    internal static class ApplicationStatusTransitionMapping
    {
        private static readonly IReadOnlyDictionary<ApplicationState, IReadOnlyCollection<ApplicationState>> StateTransitionMapping =
            new Dictionary<ApplicationState, IReadOnlyCollection<ApplicationState>>
            {
                { ApplicationState.Applied, new[] { ApplicationState.Approved, ApplicationState.Rejected } },
                { ApplicationState.Approved, new[] { ApplicationState.InReview, ApplicationState.Cancelled } },
                { ApplicationState.InReview, new[] { ApplicationState.Approved, ApplicationState.Cancelled } },
                { ApplicationState.Rejected, Array.Empty<ApplicationState>() },
                { ApplicationState.Cancelled, Array.Empty<ApplicationState>() }
            };

        public static bool CanTransitTo(this ApplicationState currentState, ApplicationState requestedState) =>
            StateTransitionMapping[currentState].Contains(requestedState);
    }
}
