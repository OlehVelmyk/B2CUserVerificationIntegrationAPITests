using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Extensions;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Utilities
{
    internal class SchemaProcessor
    {
        private readonly SchemaRepository _schemaRepository;
        private readonly ISchemaGenerator _schemaGenerator;
        private readonly StringEnumSchemaFactory _stringEnumSchemaFactory;
        private readonly List<Type> _processedTypes;

        public SchemaProcessor(SchemaRepository schemaRepository, ISchemaGenerator schemaGenerator)
        {
            _schemaRepository = schemaRepository ?? throw new ArgumentNullException(nameof(schemaRepository));
            _schemaGenerator = schemaGenerator ?? throw new ArgumentNullException(nameof(schemaGenerator));
            _stringEnumSchemaFactory = new StringEnumSchemaFactory(_schemaRepository, _schemaGenerator);
            _processedTypes = new List<Type>();
        }

        public void Process(OpenApiSchema schema, Type modelType)
        {
            var queue = CreateExecutingQueue(schema, modelType);

            while (queue.TryDequeue(out var tuple))
            {
                if (_processedTypes.Contains(tuple.ModelType))
                    continue;

                var derivedSchema = tuple.Schema.AllOf.FirstOrDefault(x => x.Type == "object");
                var typeSchema = derivedSchema ?? tuple.Schema;

                ProcessProperties(typeSchema, tuple.ModelType, queue);
                _processedTypes.Add(tuple.ModelType);
            }
        }

        private Queue<(OpenApiSchema Schema, Type ModelType)> CreateExecutingQueue(OpenApiSchema schema, Type modelType)
        {
            var queue = new Queue<(OpenApiSchema Schema, Type ModelType)>();
            ProcessSchema(schema, modelType, queue);
            return queue;
        }

        private void ProcessProperties(OpenApiSchema schema, Type modelType,
                                       Queue<(OpenApiSchema Schema, Type ModelType)> queue)
        {
            var classProperties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var realOpenApiProperties = schema.Properties;
            var openApiPropertiesCopy = schema.Properties.ToArray();
            foreach (var openApiProp in openApiPropertiesCopy)
            {
                var classProperty = classProperties.First(property => property.Name.CompareStrings(openApiProp.Key));

                if (classProperty.IsStringEnum())
                {
                    var stringEnumSchema = _stringEnumSchemaFactory.Create(openApiProp.Value, classProperty);
                    realOpenApiProperties.Remove(openApiProp.Key);
                    realOpenApiProperties.Add(openApiProp.Key, stringEnumSchema);
                }
                else
                {
                    ProcessSchema(openApiProp.Value, classProperty.PropertyType, queue);
                }

                if (classProperty.IsOptional())
                {
                    schema.Required.Remove(openApiProp.Key);
                    openApiProp.Value.Extensions.Add(new NullableExtension(true));
                    if (openApiProp.Value.Reference != null)
                    {
                        var refExtension = new RefExtension(openApiProp.Value.Reference.ReferenceV3);
                        openApiProp.Value.Extensions.Add(refExtension);
                        openApiProp.Value.Reference = null;
                    }
                }
            }
        }

        private void ProcessSchema(OpenApiSchema schema, Type modelType,
                                   Queue<(OpenApiSchema Schema, Type ModelType)> queue)
        {
            if (modelType.IsPocoClass())
                ProcessPocoClass(schema, modelType, queue);
            else if (modelType != typeof(string) && modelType.IsCollection())
            {
                var typeOfCollection = modelType.GetTypeOfCollection();
                if (typeOfCollection.IsPocoClass())
                    ProcessPocoClass(schema.Items, typeOfCollection, queue);
            }
        }

        private void ProcessPocoClass(OpenApiSchema schema, Type modelType,
                                      Queue<(OpenApiSchema Schema, Type ModelType)> queue)
        {
            if (!modelType.IsPocoClass())
                return;

            schema = GetOriginalSchema(schema);
            queue.Enqueue((schema, modelType));

            foreach (var childType in modelType.FindChildren())
            {
                var childSchema = _schemaGenerator.GenerateSchema(childType, _schemaRepository);
                childSchema = GetOriginalSchema(childSchema);
                queue.Enqueue((childSchema, childType));
            }

            OpenApiSchema GetOriginalSchema(OpenApiSchema schema)
                => schema.Reference == null ? schema : _schemaRepository.Schemas[schema.Reference.Id];
        }
    }
}
