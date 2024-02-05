using System;
using System.Collections.Generic;
using System.Text;

namespace WX.B2C.User.Verification.Worker.Jobs.Extensions
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendString(this StringBuilder builder, string value, bool fromNewLine = true)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));

            if (string.IsNullOrEmpty(value))
                return builder;

            if(fromNewLine)
                builder.Append(Environment.NewLine);

            return builder.Append(value);
        }

        public static StringBuilder Start(this StringBuilder builder, string value)
            => builder.AppendString(value, false);

        public static string End(this StringBuilder builder, string value, bool fromNewLine = true)
            => builder.AppendString(value, fromNewLine).ToString();
    }
}
