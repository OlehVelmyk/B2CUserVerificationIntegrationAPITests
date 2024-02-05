using System.Collections.Generic;
using Microsoft.AspNetCore.OData.Query;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.OData
{
    public class ODataValidationSettings : Microsoft.AspNetCore.OData.Query.Validator.ODataValidationSettings
    {
        public ISet<string> AllowedFilterProperties { get; } = new HashSet<string>();

        public bool RestrictInOperator { get; set; } = true;

        public ODataValidationSettings()
        {
            AllowedArithmeticOperators = AllowedArithmeticOperators.None;
            AllowedFunctions = AllowedFunctions.None;
            AllowedLogicalOperators = AllowedLogicalOperators.None;
            AllowedQueryOptions = AllowedQueryOptions.None;
            MaxTop = 100;
            MaxNodeCount = 10;
            MaxOrderByNodeCount = 1;
            MaxAnyAllExpressionDepth = 1;
            MaxExpansionDepth = 0;
        }
    }
}
