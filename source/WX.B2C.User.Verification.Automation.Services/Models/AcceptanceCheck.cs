using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Automation.Services
{
    public class AcceptanceCheck
    {
        public Guid VariantId { get; private set; }

        public CheckInputParameterDto[] Parameters { get; private set; }

        public TaskDto[] RelatedTasks { get; private set; }

        public static AcceptanceCheck Create(Guid variantId) => new() { VariantId = variantId };

        public AcceptanceCheck With(IEnumerable<CheckInputParameterDto> checkParameters)
        {
            Parameters = checkParameters?.ToArray() ?? throw new ArgumentNullException(nameof(checkParameters));
            return this;
        }

        public AcceptanceCheck With(IEnumerable<TaskDto> relatedTasks)
        {
            RelatedTasks = relatedTasks?.ToArray() ?? throw new ArgumentNullException(nameof(relatedTasks));
            return this;
        }
    }
}