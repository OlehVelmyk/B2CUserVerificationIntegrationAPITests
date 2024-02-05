using System;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Services;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Unit.Tests.InternalApi
{
    internal class ExternalLinkProviderTest
    {
        private IExternalLinkProvider _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = new ExternalLinkProvider();
        }

        [Test]
        [TestCase(ExternalProviderType.Onfido, "https://dashboard.onfido.com/library?_sandbox_[0]=false&q={id}")]
        [TestCase(ExternalProviderType.PassFort, "https://identity.passfort.com/profiles/{id}/applications")]

        public void ShouldReturnValidLink(ExternalProviderType provider, string linkTemplate)
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            var expectedLink = linkTemplate.Replace("{id}", id);

            // Act
            var link = _sut.Get(id, provider);

            // Assert
            link.Should().Be(expectedLink);
        }
    }
}
