using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;

namespace WX.B2C.User.Verification.Core.Contracts.Commands
{
    public class AddCollectionStepCommand
    {
        public Guid UserId { get; set; }

        public bool ForceCreating { get; set; }

        public PolicyCollectionStep CollectionStep { get; set; }
    }
}