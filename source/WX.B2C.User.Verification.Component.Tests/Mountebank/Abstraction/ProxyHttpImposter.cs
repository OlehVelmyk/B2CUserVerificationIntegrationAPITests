using System;
using System.Collections.Generic;
using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Stubs;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Factories;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Abstraction
{
    internal abstract class ProxyHttpImposter : BaseHttpImposter
    {
        private readonly Uri _proxyUrl;

        protected ProxyHttpImposter(MountebankClient client, string url, string proxyUrl) 
            : base(client, url)
        {
            _proxyUrl = new Uri(proxyUrl);
        }

        protected override IEnumerable<HttpStub> CreateBaseDefaultStubs()
        {
            var predicate = PredicateFactory.CreateBooleanPredicate(true, true, true, true, true);
            var proxyStub = new HttpStub().ReturnsProxy(_proxyUrl, ProxyMode.ProxyTransparent, new[] { predicate });
            return new[] { proxyStub };
        }
    }
}
