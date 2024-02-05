using System;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Factories
{
    internal static class TinFactory
    {
        public static string Create(TinType tinType, Seed seed)
            => tinType switch
            {
                TinType.ITIN => CreateItin(seed),
                TinType.SSN => CreateSsn(seed),
                _ => throw new ArgumentOutOfRangeException("Unsupported tin type.")
            };

        private static string CreateItin(Seed seed)
        {
            var faker = FakerFactory.Create(seed);

            var partOne = faker.Random.Number(0, 99);
            var between = faker.PickRandom(new[]
            {
                (50, 59), (60, 65), (83, 88), (90, 92), (94, 99)
            });
            var partTwo = faker.Random.Number(between.Item1, between.Item2);
            var partThree = faker.Random.Number(0, 9999);

            return "9" +
                   partOne.ToString().PadLeft(2, '0') +
                   partTwo.ToString() +
                   partThree.ToString().PadLeft(4, '0');
        }

        private static string CreateSsn(Seed seed)
        {
            var faker = FakerFactory.Create(seed);

            var partOnes = new[] { faker.Random.Number(1, 665), faker.Random.Number(667, 899) };
            var partOne = faker.PickRandom(partOnes);
            var partTwo = faker.Random.Number(1, 99);
            var partThree = faker.Random.Number(1, 9999);

            return partOne.ToString().PadLeft(3, '0') +
                   partTwo.ToString().PadLeft(2, '0') +
                   partThree.ToString().PadLeft(4, '0');
        }
    }
}
