using System;

namespace WX.B2C.User.Verification.Infrastructure.Contracts
{
    public static class CommandQueueNameResolver
    {
        private static readonly Lazy<string> Name = new(() => $"b2c-verification-commands{CommandsQueueNameSuffix()}");

        public static string Get() => Name.Value;

        private static string CommandsQueueNameSuffix()
        {
            var env = Environment.GetEnvironmentVariable(nameof(Environment));
            if (env.Equals("develop", StringComparison.InvariantCultureIgnoreCase) || env.Equals("local", StringComparison.InvariantCultureIgnoreCase))
                return $"-{Environment.MachineName}".ToLower();
            return string.Empty;
        }

    }
}