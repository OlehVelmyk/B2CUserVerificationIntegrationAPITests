using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    public class BridgerPasswordOptions
    {
        public int MaxPasswordLength { get; set; } = 20;

        public char[] SpecialChars { get; set; } = { '!', '@', '#' };
    }

    public interface IBridgerPasswordGenerator
    {
        string Generate();
    }

    internal class BridgerPasswordGenerator : IBridgerPasswordGenerator
    {
        private readonly BridgerPasswordOptions _options;

        public BridgerPasswordGenerator(BridgerPasswordOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public string Generate()
        {
            var specialChars = _options.SpecialChars;
            var special = specialChars[new Random().Next(specialChars.Length)];
            return Guid.NewGuid().ToString("N")[..(_options.MaxPasswordLength - 1)] + special;
        }
    }
}