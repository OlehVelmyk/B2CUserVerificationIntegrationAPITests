using System;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Utilities
{
    internal class StringEnumSchemaFactory
    {
        private readonly SchemaRepository _schemaRepository;
        private readonly ISchemaGenerator _schemaGenerator;

        public StringEnumSchemaFactory(SchemaRepository schemaRepository, ISchemaGenerator schemaGenerator)
        {
            _schemaRepository = schemaRepository ?? throw new ArgumentNullException(nameof(schemaRepository));
            _schemaGenerator = schemaGenerator ?? throw new ArgumentNullException(nameof(schemaGenerator));
        }

        public OpenApiSchema Create(OpenApiSchema schema, PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<StringEnumAttribute>(false);
            if (attribute == null)
                throw new InvalidOperationException("Can not create stringEnum schema.");

            return Create(schema, propertyInfo.PropertyType, attribute.EnumType);
        }

        public OpenApiSchema Create(OpenApiSchema schema, ParameterInfo parameterInfo)
        {
            var attribute = parameterInfo.GetCustomAttribute<StringEnumAttribute>(false);
            if (attribute == null)
                throw new InvalidOperationException("Can not create stringEnum schema.");

            return Create(schema, parameterInfo.ParameterType, attribute.EnumType);
        }

        private OpenApiSchema Create(OpenApiSchema schema, Type schemaType, Type enumType)
        {
            if (_schemaRepository.Schemas.ContainsKey(schemaType.Name))
                _schemaRepository.Schemas.Remove(schemaType.Name);
            if (!_schemaRepository.Schemas.ContainsKey(enumType.Name))
                _schemaGenerator.GenerateSchema(enumType, _schemaRepository);

            if (schema.Type == "array")
                schema.Items = CreateReference(enumType.Name);
            else
                schema = CreateReference(enumType.Name);

            return schema;
        }

        private OpenApiSchema CreateReference(string id)
            => new OpenApiSchema
            {
                Reference = new OpenApiReference
                {
                    Id = id,
                    Type = ReferenceType.Schema
                }
            };
    }
}
