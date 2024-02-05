using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    public class PolymorphicDocumentFilter : IDocumentFilter
    {
        private readonly Type _baseType;
        private readonly string _discriminatorName ;
        private readonly HashSet<Type> _derivedTypes;

        public PolymorphicDocumentFilter(Type baseType, HashSet<Type> derivedTypes, string discriminatorName)
        {
            _baseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
            _derivedTypes = derivedTypes ?? throw new ArgumentNullException(nameof(derivedTypes));
            _discriminatorName = discriminatorName ?? throw new ArgumentNullException(nameof(discriminatorName));
        }

        public void Apply(OpenApiDocument openApiDoc, DocumentFilterContext context)
        {
            var schemaRepository = context.SchemaRepository.Schemas;
            var schemaGenerator = context.SchemaGenerator;

            if (!schemaRepository.TryGetValue(_baseType.Name, out var parentSchema))
            {
                parentSchema = schemaGenerator.GenerateSchema(_baseType, context.SchemaRepository);
            }

            // set up a discriminator property (it must be required)
            parentSchema.Discriminator = new OpenApiDiscriminator { PropertyName = _discriminatorName };
            parentSchema.Required.Add(_discriminatorName);

            if (!parentSchema.Properties.ContainsKey(_discriminatorName))
                parentSchema.Properties.Add(_discriminatorName, new OpenApiSchema { Type = "string", Default = new OpenApiString(_baseType.FullName) });

            // register all subclasses
            foreach (var type in _derivedTypes)
                schemaGenerator.GenerateSchema(type, context.SchemaRepository);
        }
    }
}
