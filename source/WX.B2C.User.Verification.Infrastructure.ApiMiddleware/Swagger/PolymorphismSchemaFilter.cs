using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    public class PolymorphicSchemaFilter : ISchemaFilter
    {
        private readonly Type _baseType;
        private readonly HashSet<Type> _derivedTypes;
        private readonly Func<Type, string> _discriminatorProvider;

        public PolymorphicSchemaFilter(Type baseType, HashSet<Type> derivedTypes, Func<Type, string> discriminatorProvider)
        {
            _baseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
            _derivedTypes = derivedTypes ?? throw new ArgumentNullException(nameof(baseType));
            _discriminatorProvider = discriminatorProvider ?? throw new ArgumentNullException(nameof(discriminatorProvider));
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (!_derivedTypes.Contains(type))
                return;

            schema.AllOf = new List<OpenApiSchema>
            {
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Id = _baseType.Name,
                        Type = ReferenceType.Schema
                    }
                },
                new OpenApiSchema
                {
                    Type = schema.Type,
                    Properties = schema.Properties,
                    Required = schema.Required
                }
            };

            var discriminatorValue = _discriminatorProvider(type);
            schema.AddExtension("x-ms-discriminator-value", new OpenApiString(discriminatorValue));

            // reset properties for they are included in allOf, should be null but code does not handle it
            schema.Properties = new Dictionary<string, OpenApiSchema>();
            schema.Required = new HashSet<string>();
        }
    }
}