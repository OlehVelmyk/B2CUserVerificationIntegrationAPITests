using System;
using System.Collections.Generic;

namespace WX.B2C.User.Verification.Provider.Contracts.Models
{
    public class CheckInputData
    {
        public CheckInputData(Guid userId, string externalProfileId, IReadOnlyDictionary<string, object> data)
        {
            UserId = userId;
            ExternalProfileId = externalProfileId;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
        
        public Guid UserId { get; }

        public string ExternalProfileId { get; }

        public IReadOnlyDictionary<string, object> Data { get; }

        public bool TryGetValue<T>(string xPath, out T value)
        {
            if (string.IsNullOrWhiteSpace(xPath))
                throw new ArgumentNullException(nameof(xPath));

            value = default;

            if (!Data.ContainsKey(xPath))
                return false;

            value = (T)Data[xPath];
            return true;
        }

        public T TryGetValue<T>(string xPath, IList<string> validationErrors)
        {
            if (string.IsNullOrWhiteSpace(xPath))
                throw new ArgumentNullException(nameof(xPath));
            if (validationErrors == null)
                throw new ArgumentNullException(nameof(validationErrors));

            var hasValue = TryGetValue(xPath, out T value);
            if (hasValue) return value;

            validationErrors.Add(xPath);
            return default;
        }
    }
}