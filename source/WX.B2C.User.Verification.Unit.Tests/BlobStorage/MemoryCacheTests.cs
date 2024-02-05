using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.BlobStorage.Cache;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Unit.Tests.BlobStorage
{
    [TestFixture]
    public class MemoryCacheTests
    {
        private ISystemClock _systemClock;
        private IMemoryCache _cache;

        [SetUp]
        public void Setup()
        {
            _systemClock = Substitute.For<ISystemClock>();
            _cache = new MemoryCache(new InMemoryProvider(TimeSpan.FromDays(1), _systemClock));
        }

        [Test]
        public async Task ShouldStoreItemInCache()
        {
            var first = Guid.NewGuid();
            var second = Guid.NewGuid();

            _systemClock.GetDate().Returns(DateTime.Now);
            await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(first));
            var secondReadingResult = await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(second));

            secondReadingResult.Should().Be(first);
        }

        [Test]
        public async Task ShouldStoreItemInCacheDifferentTypes()
        {
            var expectedGuid = Guid.NewGuid();
            var expectedOption = new CountriesOption(new[] { new CountryOption("test", "test", "test", Array.Empty<StateOption>()) });
            var expectedString = "test";

            _systemClock.GetDate().Returns(DateTime.Now);

            await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(expectedGuid));
            var actualGuid = await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(expectedGuid));
            await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(expectedOption));
            var actualOption = await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(expectedOption));
            await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(expectedString));
            var actualString = await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(expectedString));

            actualGuid.Should().Be(expectedGuid);
            actualOption.Should().Be(expectedOption);
            actualString.Should().Be(expectedString);
        }

        [Test]
        public async Task ShouldReloadItemFromFactory_WhenItIsExpired()
        {
            var first = Guid.NewGuid();
            var second = Guid.NewGuid();

            _systemClock.GetDate().Returns(DateTime.Now);
            await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(first));
            _systemClock.GetDate().Returns(DateTime.Now.AddDays(1));
            var secondReadingResult = await _cache.GetOrCreate(TimeSpan.FromDays(1), () => Task.FromResult(second));

            secondReadingResult.Should().Be(second);
        }
    }
}