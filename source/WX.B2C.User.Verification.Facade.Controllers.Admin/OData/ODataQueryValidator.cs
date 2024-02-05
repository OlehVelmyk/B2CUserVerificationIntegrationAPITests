using System;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.OData
{
    internal class ODataQueryValidator : Microsoft.AspNetCore.OData.Query.Validator.ODataQueryValidator
    {
        public void Validate(ODataQueryOptions options, ODataValidationSettings validationSettings)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));
            if (validationSettings is null)
                throw new ArgumentNullException(nameof(validationSettings));

            ValidateQueryOption(options);
            base.Validate(options, validationSettings);
        }

        /// <summary>
        /// Validation is duplicated from <see cref="EnableQueryAttribute.ValidateQuery"/>
        /// </summary>
        private void ValidateQueryOption(ODataQueryOptions options)
        {
            var queryParameters = options.Request?.Query;
            if (queryParameters is null)
                throw new ArgumentNullException($"{nameof(ODataQueryOptions.Request.Query)} can not be null.");

            foreach (var kvp in queryParameters)
            {
                if (!options.IsSupportedQueryOption(kvp.Key) && kvp.Key.StartsWith("$", StringComparison.Ordinal))
                    throw new ODataException($"Custom query option '{kvp.Key}' that starts with '$' is not supported.");
            }
        }
    }
}
