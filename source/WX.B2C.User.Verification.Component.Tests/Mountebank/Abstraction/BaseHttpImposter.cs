using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MbDotNet;
using MbDotNet.Models.Imposters;
using MbDotNet.Models.Stubs;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Abstraction
{
    internal abstract class BaseHttpImposter : IHttpImposter
    {
        private const string UrlRegex = @"^http:[/]{2}localhost:\d+[/]{0,1}.*$";
        
        private readonly MountebankClient _client;
        private readonly int _port;

        public bool IsActive => _client.Imposters.FirstOrDefault(i => i.Port == _port) is HttpImposter;

        public BaseHttpImposter(MountebankClient client, string url)
        {
            if (!Regex.IsMatch(url, UrlRegex))
                throw new ArgumentException(nameof(url));

            _client = client ?? throw new ArgumentNullException(nameof(client));
            _port = int.TryParse(url.Split(":").Last().Split("/").First(), out var p) ? p : 80;
        }

        public virtual Task ConfigureDefaultAsync()
        {
            var stubs = new List<HttpStub>();
            stubs.AddRange(CreateCustomDefaultStubs());
            stubs.AddRange(CreateBaseDefaultStubs());

            return AppendStubsAsync(stubs);
        }

        public async Task ResetAsync()
        {
            await RemoveAsync();
            await ConfigureDefaultAsync();
        }

        public Task RemoveAsync() =>
            _client.DeleteImposterAsync(_port);

        protected virtual IEnumerable<HttpStub> CreateCustomDefaultStubs() => Array.Empty<HttpStub>();

        protected virtual IEnumerable<HttpStub> CreateBaseDefaultStubs() => Array.Empty<HttpStub>();

        protected Task AppendStubAsync(HttpStub stub) =>
            _client.AppendStubAsync(stub, _port);

        protected Task AppendStubsAsync(IEnumerable<HttpStub> stubs) =>
            _client.AppendStubsAsync(stubs, _port);
    }
}
