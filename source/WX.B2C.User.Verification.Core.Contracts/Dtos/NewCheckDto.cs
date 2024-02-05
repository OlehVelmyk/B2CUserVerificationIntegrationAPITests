using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class NewCheckDto
    {
        public Guid? Id { get; set; }

        public CheckType CheckType { get; set; }

        public Guid VariantId { get; set; }

        // TODO: Could be removed
        public CheckProviderType Provider { get; set; }

        public Guid[] RelatedTasks { get; set; }
    }
}
