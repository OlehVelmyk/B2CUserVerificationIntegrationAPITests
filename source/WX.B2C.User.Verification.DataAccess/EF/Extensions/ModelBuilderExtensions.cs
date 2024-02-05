using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WX.B2C.User.Verification.DataAccess.EF.Extensions
{
    internal static class ModelBuilderExtensions
    {
        internal static ModelBuilder ConfigureDateTimeKind(this ModelBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                value => value,
                dateTime => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                value => value,
                time => time.HasValue ? DateTime.SpecifyKind(time.Value, DateTimeKind.Utc) : time);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                        property.SetValueConverter(dateTimeConverter);
                    else if (property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(nullableDateTimeConverter);
                }
            }

            return builder;
        }
    }
}
