using Microsoft.OData;
using Microsoft.OData.UriParser;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.OData
{
    internal class FilterQueryValidator : Microsoft.AspNetCore.OData.Query.Validator.FilterQueryValidator
    {
        protected override void ValidateSingleValuePropertyAccessNode(
            SingleValuePropertyAccessNode propertyAccessNode,
            Microsoft.AspNetCore.OData.Query.Validator.ODataValidationSettings settings)
        {
            if (settings is ODataValidationSettings validationSettings)
            {
                var propertyName = propertyAccessNode?.Property.Name;
                if (propertyName != null && !validationSettings.AllowedFilterProperties.Contains(propertyName))
                    throw new ODataException($"Filter on {propertyName} not allowed");
            }

            base.ValidateSingleValuePropertyAccessNode(propertyAccessNode, settings);
        }

        protected override void ValidateQueryNode(
            QueryNode node, 
            Microsoft.AspNetCore.OData.Query.Validator.ODataValidationSettings settings)
        {
            if (settings is ODataValidationSettings validationSettings)
            {
                if (validationSettings.RestrictInOperator && node?.Kind is QueryNodeKind.In)
                    throw new ODataException($"In operator is not allowed.");
            }

            base.ValidateQueryNode(node, settings);
        }
    }
}
