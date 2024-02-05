using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Optional;
using Quartz;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Extensions
{
    internal static class JobDataExtensions
    {
        public static JobSettings GetSettings(this IJobExecutionContext context) =>
            context.GetSettings<JobSettings>();

        public static TSetting GetSettings<TSetting>(this IJobExecutionContext context) =>
            context.Get<TSetting>(Constants.JobSettings);

        private static TResult Get<TResult>(this IJobExecutionContext context, string key)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var serializedData = context.MergedJobDataMap.GetString(key);
            if (serializedData == null)
                throw new ArgumentNullException(nameof(serializedData));

            return JsonConvert.DeserializeObject<TResult>(serializedData, new OptionConverter());
        }
    }

    internal class OptionConverter : JsonConverter
    {
        private static MethodInfo CreateOptionMethod;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override object ReadJson(JsonReader reader,
                                        Type objectType,
                                        object existingValue,
                                        JsonSerializer serializer)
        {
            var valueType = objectType.GetGenericArguments().Single();
            var factory = GetFactory(valueType);

            var value = GetValue(reader.Value, valueType);
            var result = factory.Invoke(null, new[] { value });
            return result;
        }

        private static object GetValue(object value, Type valueType)
        {
            if (value == null)
                return null;

            if (valueType.IsEnum)
                return Enum.Parse(valueType, value.ToString()!);

            if (valueType.IsGenericType)
            {
                var definition = valueType.GetGenericTypeDefinition();
                if (definition == typeof(Nullable<>))
                {
                    var parameterType = valueType.GetGenericArguments().First();
                    return GetValue(value, parameterType);
                }

                throw new NotImplementedException();
            }

            return value;
        }

        private static MethodInfo GetFactory(Type valueType)
        {
            CreateOptionMethod ??= typeof(Option).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                 .First(info => info.Name == nameof(Option.Some));

            var factory = CreateOptionMethod.MakeGenericMethod(valueType);
            return factory;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsGenericType)
                return objectType.GetGenericTypeDefinition() == typeof(Option<>);

            return false;
        }
    }
}