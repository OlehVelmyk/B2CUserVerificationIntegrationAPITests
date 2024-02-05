using System.Collections.Generic;
using System.Net;
using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Stubs;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Factories;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Abstraction
{
    internal abstract class DefaultHttpImposter : BaseHttpImposter
    {
        protected DefaultHttpImposter(MountebankClient client, string url) 
            : base(client, url)
        {  }

        protected override IEnumerable<HttpStub> CreateBaseDefaultStubs()
        {
            var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Get);
            var defaultGetStub = new HttpStub().On(predicate).ReturnsStatus(HttpStatusCode.NotFound);

            var defaultStub = new HttpStub().ReturnsStatus(HttpStatusCode.Forbidden);
            return new[] { defaultGetStub, defaultStub };
        }
    }
}
