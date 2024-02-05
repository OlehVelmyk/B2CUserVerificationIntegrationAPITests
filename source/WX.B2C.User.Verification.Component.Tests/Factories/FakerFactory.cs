using Bogus;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Factories
{
    internal static class FakerFactory
    {
        private static Faker Instance;
        private static readonly object LockObject = new();

        public static Faker Create(Seed seed)
        {
            if (Instance is null)
            {
                lock (LockObject)
                {
                    Instance = new Faker
                    {
                        Random = new Randomizer(seed)
                    };
                }
            }
            return Instance;
        }
    }
}
