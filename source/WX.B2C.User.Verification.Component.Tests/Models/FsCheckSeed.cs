using System.Linq;
using FsCheck;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class FsCheckSeed
    {
        public Rnd Value { get; private set; }

        public void Update(Rnd seed) => Value = seed;

        public override string ToString() => $"({Value.Seed},{Value.Gamma})";

        public static FsCheckSeed Create(Rnd seed) =>
            new FsCheckSeed
            {
                Value = seed
            };

        public static FsCheckSeed Parse(string seed)
        {
            seed = seed.Trim('(', ')');
            var parts = seed.Split(',')
                            .Select(part => ulong.Parse(part))
                            .ToArray();
            var rnd = new Rnd(parts[0], parts[1]);
            return Create(rnd);
        }

        public static implicit operator Rnd(FsCheckSeed seed) => seed.Value;
    }
}
