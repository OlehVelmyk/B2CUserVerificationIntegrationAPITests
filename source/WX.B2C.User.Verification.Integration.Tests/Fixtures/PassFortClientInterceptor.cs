using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Rest;
using WX.B2C.User.Verification.PassFort.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Fixtures
{
    internal class PassFortClientInterceptor : IServiceClientTracingInterceptor
    {
        private readonly List<HttpRequestMessage> _sentUpdateCollectedDataRequests = new();
        private readonly List<HttpOperationResponse<IndividualData, ProfilesUpdateCollectedDataHeaders>> _responses = new();

        public void Configuration(string source, string name, string value) { }

        public void EnterMethod(string invocationId,
                                object instance,
                                string method,
                                IDictionary<string, object> parameters) { }

        public void ExitMethod(string invocationId, object returnValue)
        {
            if (returnValue is HttpOperationResponse<IndividualData, ProfilesUpdateCollectedDataHeaders> response)
                _responses.Add(response);
        }

        public void Information(string message) { }

        public void ReceiveResponse(string invocationId, HttpResponseMessage response) { }

        public void SendRequest(string invocationId, HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Post && request.RequestUri.Segments.Last() == "collected_data")
                _sentUpdateCollectedDataRequests.Add(request);
        }

        public void TraceError(string invocationId, Exception exception) { }

        public IReadOnlyCollection<HttpRequestMessage> UpdateCollectedDataRequests => _sentUpdateCollectedDataRequests;

        public IReadOnlyCollection<HttpOperationResponse<IndividualData, ProfilesUpdateCollectedDataHeaders>>
            UpdateCollectedDataResponses => _responses;
    }
}