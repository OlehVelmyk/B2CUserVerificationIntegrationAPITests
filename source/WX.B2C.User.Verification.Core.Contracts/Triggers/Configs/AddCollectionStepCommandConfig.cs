using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Triggers.Configs
{
    public class AddCollectionStepCommandConfig : CommandConfig
    {
        public TaskType TaskType { get; set; }

        public PolicyCollectionStep CollectionStep { get; set; }
    }
}