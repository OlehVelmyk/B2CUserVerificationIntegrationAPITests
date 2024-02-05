using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MbDotNet;
using MbDotNet.Models.Imposters;
using MbDotNet.Models.Stubs;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Extensions
{
    internal static class MountebankClientExtensions
    {
        public static Task AppendStubAsync(this MountebankClient client, HttpStub stub, int port) =>
            client.AppendStubsAsync(new[] { stub ?? throw new ArgumentNullException(nameof(stub)) }, port);

        public static Task AppendStubsAsync(this MountebankClient client, IEnumerable<HttpStub> stubs, int port)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));
            if (stubs is null)
                throw new ArgumentNullException(nameof(stubs));
            if (!stubs.Any())
                return Task.CompletedTask;

            var imposter = client.Imposters.FirstOrDefault(i => i.Port == port) as HttpImposter;
            if (imposter == null)
            {
                imposter = client.CreateHttpImposter(port);
                imposter.Stubs.AddRange(stubs);
                return client.SubmitAsync(imposter);
            }

            imposter.AppendStubs(stubs);
            return client.UpdateImposterAsync(imposter);
        }
    }
}
