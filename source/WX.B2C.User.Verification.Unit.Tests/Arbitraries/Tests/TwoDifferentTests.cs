using System;
using FluentAssertions;
using FsCheck;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.Tests
{
    internal class TwoDifferentTests
    {
        public TwoDifferentTests()
        {
            Arb.Register<TwoDifferentArbitrary<int>>();
            Arb.Register<TwoDifferentArbitrary<Guid>>();

            Arb.Register<ThreeDifferentArbitrary<int>>();
            Arb.Register<ThreeDifferentArbitrary<Guid>>();
        }

        [Theory(MaxTest = 100)]
        public void ShouldBeTwoDifferentIntValues(TwoDifferent<int> values)
        {
            values.Item1.Should().NotBe(values.Item2);
        }

        [Theory(MaxTest = 100)]
        public void ShouldBeTwoDifferentGuidValues(TwoDifferent<Guid> values)
        {
            values.Item1.Should().NotBe(values.Item2);
        }

        [Theory(MaxTest = 100)]
        public void ShouldBeThreeDifferentIntValues(ThreeDifferent<int> values)
        {
            values.Item2.Should().NotBe(values.Item1);
            values.Item3.Should().NotBe(values.Item1);
            values.Item3.Should().NotBe(values.Item2);
        }

        [Theory(MaxTest = 100)]
        public void ShouldBeThreeDifferentGuidValues(ThreeDifferent<Guid> values)
        {
            values.Item2.Should().NotBe(values.Item1);
            values.Item3.Should().NotBe(values.Item1);
            values.Item3.Should().NotBe(values.Item2);
        }
    }
}
