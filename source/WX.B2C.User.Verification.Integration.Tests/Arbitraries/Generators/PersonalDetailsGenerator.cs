using Bogus;
using Bogus.DataSets;
using FsCheck;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators
{
    internal static class PersonalDetailsGenerator
    {
        public static Gen<string> FirstName(int seed, string locale = "en")
        {
            var nameDataSet = CreateNameDataSet(seed, locale);
            return Gen.Constant(nameDataSet.FirstName());
        }

        public static Gen<string> LastName(int seed, string locale = "en")
        {
            var nameDataSet = CreateNameDataSet(seed, locale);
            return Gen.Constant(nameDataSet.LastName());
        }

        public static Gen<string> FullName(int seed, string locale = "en")
        {
            var nameDataSet = CreateNameDataSet(seed, locale);
            return Gen.Constant(nameDataSet.FullName());
        }

        public static Gen<string> Email(int seed, string locale = "en")
        {
            var dataSet = CreateInternetDataSet(seed, locale);
            return Gen.Constant(dataSet.Email());
        }

        private static Name CreateNameDataSet(int seed, string locale = "en")
            => new()
            {
                Random = new Randomizer(seed),
                Locale = locale
            };

        private static Internet CreateInternetDataSet(int seed, string locale = "en")
            => new()
            {
                Random = new Randomizer(seed),
                Locale = locale
            };
    }
}
