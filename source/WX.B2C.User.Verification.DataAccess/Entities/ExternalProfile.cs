using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    /// <summary>
    /// Can be extended if we need to store additional information from provider
    /// </summary>
    internal class ExternalProfile
    {
        public Guid UserId { get; set; }

        public ExternalProviderType Provider { get; set; }

        public string ExternalId { get; set; }
    }
}