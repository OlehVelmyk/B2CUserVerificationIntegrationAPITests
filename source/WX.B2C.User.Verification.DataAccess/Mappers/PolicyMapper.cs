using System;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Conditions;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IPolicyMapper
    {
        VerificationPolicyDto Map(VerificationPolicy verificationPolicy);

        CheckVariantInfo Map(PolicyCheckVariant checkVariant);

        CheckFailPolicy Map(CheckResultType checkResultType, string failResult, string failResultCondition);

        TaskVariantDto Map(TaskVariant taskVariant);

        VariantNameDto Map(Guid variantId, string name);
    }

    internal class PolicyMapper : IPolicyMapper
    {
        private readonly IPolicyObjectsDeserializer _policyObjectsDeserializer;
        private readonly IXPathParser _xPathParser;

        public PolicyMapper(IPolicyObjectsDeserializer policyObjectsDeserializer, IXPathParser xPathParser)
        {
            _policyObjectsDeserializer = policyObjectsDeserializer ?? throw new ArgumentNullException(nameof(policyObjectsDeserializer));
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
        }

        public VerificationPolicyDto Map(VerificationPolicy verificationPolicy)
        {
            var tasks = verificationPolicy.Tasks.Select(task => Map(task.TaskVariant)).ToArray();

            return new VerificationPolicyDto
            {
                Id = verificationPolicy.Id,
                Tasks = tasks,
                RejectionPolicy = verificationPolicy.RejectionPolicy
            };
        }

        public TaskVariantDto Map(TaskVariant taskVariant)
        {
            if (taskVariant == null)
                throw new ArgumentNullException(nameof(taskVariant));

            return new TaskVariantDto
            {
                VariantId = taskVariant.Id,
                TaskName = taskVariant.Name,
                Type = taskVariant.Type,
                Priority = taskVariant.Priority,
                CheckVariants = taskVariant.ChecksVariants?.Select(variant => variant.CheckVariantId).ToArray() ?? Array.Empty<Guid>(),
                AutoCompletePolicy = taskVariant.AutoCompletePolicy,
                CollectionSteps = taskVariant.CollectionSteps ?? Array.Empty<PolicyCollectionStep>(),
            };
        }

        public CheckFailPolicy Map(CheckResultType checkResultType, string failResult, string failResultCondition)
        {
            var condition = failResultCondition != null
                ? Deserialize<Condition>(failResultCondition)
                : null;

            return checkResultType switch
            {
                CheckResultType.AddCollectionStep => new AddCollectionStepFailResult(condition, CreatePolicyCollectionStep(failResult)),
                CheckResultType.ResubmitCollectionStep => new ResubmitCollectionStepFailResult(condition, CreatePolicyCollectionStep(failResult)),
                CheckResultType.RunCheck => new InstructCheckFailResult(condition, Deserialize<Guid>(failResult)),
                _ => throw new ArgumentOutOfRangeException(nameof(checkResultType), checkResultType, "Unsupported check fail policy.")
            };
        }

        public CheckVariantInfo Map(PolicyCheckVariant checkVariant)
        {
            if (checkVariant == null)
                throw new ArgumentNullException(nameof(checkVariant));

            return new CheckVariantInfo
            {
                Type = checkVariant.Type,
                Id = checkVariant.Id,
                Provider = checkVariant.Provider,
                MaxAttempts = checkVariant.RunPolicy?.MaxAttempts
            };
        }

        public VariantNameDto Map(Guid variantId, string name) => new() { Id = variantId, Name = name };

        private PolicyCollectionStep CreatePolicyCollectionStep(string failResult)
        {
            var stepVariant = Deserialize<PolicyCollectionStep>(failResult);
            if (!_xPathParser.IsValid(stepVariant.XPath))
                throw new InvalidOperationException($"XPath {stepVariant.XPath} is not valid in check variant fail result.");

            return stepVariant;
        }

        private T Deserialize<T>(string json) => _policyObjectsDeserializer.Deserialize<T>(json);
    }
}
