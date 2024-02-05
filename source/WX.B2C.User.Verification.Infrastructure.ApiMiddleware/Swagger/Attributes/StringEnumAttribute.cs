using System;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class StringEnumAttribute : Attribute
    {
        public Type EnumType { get; private set; }

        public StringEnumAttribute(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum)
                throw new InvalidOperationException($"Parameter { nameof(enumType) } is not type of enum.");

            EnumType = enumType;
        }
    }
}
