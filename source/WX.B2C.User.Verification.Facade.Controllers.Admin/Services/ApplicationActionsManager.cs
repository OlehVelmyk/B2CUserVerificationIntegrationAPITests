using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Services
{
    internal interface IApplicationActionsManager
    {
        IEnumerable<ApplicationAction> GetAllowedActions(ApplicationDto application);
    }

    internal class ApplicationActionsManager : IApplicationActionsManager
    {
        private static readonly ApplicationAction[] ApprovalActions = { ApplicationAction.Approve, ApplicationAction.Review };

        public IEnumerable<ApplicationAction> GetAllowedActions(ApplicationDto application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            var allowedActions = GetActionsMap(application.State);
            allowedActions = FilterApprovalActions(allowedActions, application);
            return allowedActions.ToArray();
        }

        private static IEnumerable<ApplicationAction> FilterApprovalActions(
            IEnumerable<ApplicationAction> allowedActions,
            ApplicationDto application)
        {
            var allTasksPassed = application.Tasks.All(x => x.Result == TaskResult.Passed);
            if (!allTasksPassed)
                allowedActions = allowedActions.Where(action => !ApprovalActions.Contains(action));

            return allowedActions;
        }

        private static IEnumerable<ApplicationAction> GetActionsMap(ApplicationState state)
        {
            return state switch
            {
                ApplicationState.Applied   => new[] { ApplicationAction.Approve, ApplicationAction.Reject },
                ApplicationState.Approved  => new[] { ApplicationAction.Review, ApplicationAction.Reject },
                ApplicationState.InReview  => new[] { ApplicationAction.Approve, ApplicationAction.Reject },
                ApplicationState.Rejected  => new[] { ApplicationAction.RevertDecision },
                ApplicationState.Cancelled => new[] { ApplicationAction.RevertDecision },
                _                          => Array.Empty<ApplicationAction>()
            };
        }
    }
}