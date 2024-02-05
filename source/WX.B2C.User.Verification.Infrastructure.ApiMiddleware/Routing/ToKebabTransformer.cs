using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Routing
{
    public class ToKebabTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            if (value == null)
                return null;

            var result = Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
            return result;
        }
    }
}
