using System;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class NotRequiredAttribute : Attribute
    {
    }
}