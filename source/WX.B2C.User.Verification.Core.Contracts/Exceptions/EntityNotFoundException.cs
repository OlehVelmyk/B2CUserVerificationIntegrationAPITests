using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Exceptions;

namespace WX.B2C.User.Verification.Core.Contracts.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : B2CVerificationException
    {
        public EntityNotFoundException(string entityName, string key)
            : base($"Entity {entityName} is not found by key: {key}.")
        {
        }

        public EntityNotFoundException(string entityName, IEnumerable<Guid> keys)
            : base($"Entities {entityName} are not found by keys: {FormatKeys(keys)}.")
        {
        }

        public EntityNotFoundException(string entityName, object query)
            : base($"Entities {entityName} are not found by query: {FormatQuery(query)}.")
        {
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static EntityNotFoundException ByKey<T>(Guid key) => ByKey<T>(key.ToString());

        public static EntityNotFoundException ByKey<T>(string key) => new(typeof(T).Name, key);

        public static EntityNotFoundException ByKeys<T>(IEnumerable<Guid> keys) => new(typeof(T).Name, keys);

        public static EntityNotFoundException ByQuery<T>(object query) => new(typeof(T).Name, query);

        private static string FormatKeys(IEnumerable<Guid> keys) => string.Join(",", keys);

        private static string FormatQuery(object query)
        {
            if (query == null)
                return string.Empty;

            var properties = query.GetType().GetProperties();
            var formattedProperties = properties.Select(p => $"{p.Name}:{p.GetValue(query)}");
            return string.Join(",", formattedProperties);
        }
    }
}