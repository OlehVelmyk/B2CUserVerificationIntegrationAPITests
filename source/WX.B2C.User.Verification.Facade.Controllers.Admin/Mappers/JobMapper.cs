using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using Schedule = WX.B2C.User.Verification.Core.Contracts.Schedule;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IJobMapper
    {
        JobRequestDto Map(Requests.ScheduleJobRequest request);

        JobParametersDto Map(UnscheduleJobRequest request);
    }

    internal class JobMapper : IJobMapper
    {
        public JobRequestDto Map(Requests.ScheduleJobRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new JobRequestDto
            {
                JobName = request.JobName,
                StartAt = request.StartAt,
                Schedule = Map(request.Schedule),
                Parameters = request.JobParameters?.Select(Map).ToArray()
            };
        }

        private static Schedule Map(Requests.JobSchedule requestSchedule) =>
            requestSchedule switch
            {
                CronJobSchedule cronSchedule => new Core.Contracts.CronSchedule { Cron = cronSchedule.Cron },
                IntervalJobSchedule intervalSchedule => new Core.Contracts.IntervalSchedule
                {
                    Unit = intervalSchedule.Unit,
                    Value = intervalSchedule.Value
                },
                null => null,
                _    => throw new ArgumentOutOfRangeException(nameof(requestSchedule))
            };

        public JobParametersDto Map(UnscheduleJobRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new JobParametersDto
            {
                JobName = request.JobName,
                Parameters = request.JobParameters?.Select(Map).ToArray()
            };
        }

        private static JobParameterDto Map(KeyValuePair<string, object> parameter)
        {
            var value = parameter.Value is JsonElement element ? ExtractValue(element) : parameter.Value;
            return new JobParameterDto
            {
                Name = parameter.Key,
                Value = value
            };
        }

        private static object ExtractValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    if (element.TryGetDateTime(out var date))
                        return date;
                    if (element.TryGetGuid(out var id))
                        return id;
                    return element.GetString();
                case JsonValueKind.Number:
                    return element.GetInt32();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Array:
                    return element.EnumerateArray().Select(ExtractValue).ToArray();
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                    return ExtractObject(element);
                default:
                    throw new ArgumentOutOfRangeException(nameof(element.ValueKind), element.ValueKind, "Cannot convert to object");
            }
        }

        private static object ExtractObject(JsonElement element)
        {
            var objectEnumerator = element.EnumerateObject();
            var result = new Dictionary<string, object>();
            foreach (var jsonProperty in objectEnumerator)
            {
                result[jsonProperty.Name] = ExtractValue(jsonProperty.Value);
            }

            return result;
        }
    }
}