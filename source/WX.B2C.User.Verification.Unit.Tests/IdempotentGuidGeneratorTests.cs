using System;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Infrastructure.Common;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;

namespace WX.B2C.User.Verification.Unit.Tests
{
    internal class IdempotentGuidGeneratorTests
    {
        private readonly IIdempotentGuidGenerator _guidGenerator;
        private readonly IOperationContextProvider _operationContextProvider;

        public IdempotentGuidGeneratorTests()
        {
            _operationContextProvider = Substitute.For<IOperationContextProvider>();
            _guidGenerator = new IdempotentGuidGenerator(_operationContextProvider);

            Arb.Register<TwoDifferentArbitrary<int>>();
            Arb.Register<TwoDifferentArbitrary<Guid>>();
        }

        [Test]
        [TestCase("7345FAE5-51D1-4304-B41D-993FCB669083", 1230479318, "C7B092E2-3238-5DA9-4A7D-F821AF07C8C5")]
        [TestCase("94F3FBEB-2226-4D6D-BDF6-FF665290C5A9", -2032770037, "04734ABE-62AF-316E-1930-A16412F1A629")]
        [TestCase("A13C6C60-7021-4CD5-8166-7D6893378146", 2015029530, "7C0C0BAA-C20A-3B5A-F6CF-85A4809984AA")]
        public void ShouldBeDeterministic(string correlationId, int seed, string expected)
        {
            var operationContext = OperationContext.Create(Guid.Parse(correlationId), null, Guid.NewGuid(), null);
            _operationContextProvider.GetContextOrDefault().Returns(operationContext);

            var actual = _guidGenerator.Generate(seed);

            actual.Should().Be(expected);
        }

        [Theory]
        public void ShouldRelyOnCorrelationId(TwoDifferent<Guid> correlationIds, int bias)
        {
            var operationContext = OperationContext.Create(correlationIds.Item1, null, Guid.Empty, null);
            _operationContextProvider.GetContextOrDefault().Returns(operationContext);
            var firstGuid = _guidGenerator.Generate(bias);

            operationContext = OperationContext.Create(correlationIds.Item2, null, Guid.Empty, null);
            _operationContextProvider.GetContextOrDefault().Returns(operationContext);
            var secondGuid = _guidGenerator.Generate(bias);

            firstGuid.Should().NotBe(secondGuid);
        }

        [Theory]
        public void ShouldRelyOnBias(Guid correlationId, TwoDifferent<int> bias)
        {
            var operationContext = OperationContext.Create(correlationId, null, Guid.Empty, null);
            _operationContextProvider.GetContextOrDefault().Returns(operationContext);

            var firstGuid = _guidGenerator.Generate(bias.Item1);
            var secondGuid = _guidGenerator.Generate(bias.Item2);

            firstGuid.Should().NotBe(secondGuid);
        }
    }
}
