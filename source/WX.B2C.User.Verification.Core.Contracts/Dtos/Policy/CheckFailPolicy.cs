using System;
using WX.B2C.User.Verification.Core.Contracts.Conditions;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Policy
{
    public abstract class CheckFailPolicy
    {
        protected CheckFailPolicy(Condition condition)
        {
            Condition = condition;
        }

        public Condition Condition { get; }
    }

    public class AddCollectionStepFailResult : CheckFailPolicy
    {
        public AddCollectionStepFailResult(Condition condition, PolicyCollectionStep step)
            : base(condition)
        {
            Step = step ?? throw new ArgumentNullException(nameof(step));
        }

        public PolicyCollectionStep Step { get; }
    }

    public class ResubmitCollectionStepFailResult : CheckFailPolicy
    {
        public ResubmitCollectionStepFailResult(Condition condition, PolicyCollectionStep step)
            : base(condition)
        {
            Step = step ?? throw new ArgumentNullException(nameof(step));
        }

        public PolicyCollectionStep Step { get; }
    }

    public class InstructCheckFailResult : CheckFailPolicy
    {
        public InstructCheckFailResult(Condition condition, Guid variantId)
            : base(condition)
        {
            VariantId = variantId;
        }

        public Guid VariantId { get; }
    }
}
