using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Commands
{
    public class AddCollectionStepsToTaskCommand
    {
        public Guid ApplicationId { get; set; }

        public TaskType TaskType { get; set; }

        public Guid[] CollectionSteps { get; set; }
    }
}