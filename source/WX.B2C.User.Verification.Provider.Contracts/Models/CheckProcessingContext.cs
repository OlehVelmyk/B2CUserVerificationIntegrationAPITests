using System;
using System.Collections.Generic;

namespace WX.B2C.User.Verification.Provider.Contracts.Models
{
    public class CheckProcessingContext
    {
        public CheckProcessingContext(IReadOnlyDictionary<string, object> externalData)
        {
            ExternalData = externalData ?? throw new ArgumentNullException(nameof(externalData));
        }

        public IReadOnlyDictionary<string, object> ExternalData { get; }

        public static CheckProcessingContext Create(IReadOnlyDictionary<string, object> externalData) => new(externalData);
    }
}