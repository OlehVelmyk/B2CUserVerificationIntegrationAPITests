using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;

namespace WX.B2C.User.Verification.Integration.Tests.Core.DelegatingHandlers;

internal class LogDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        StepContext.Instance[General.CorrelationId] = correlationId.ToString();

        request.Headers.Remove(General.CorrelationId);
        request.Headers.Add(General.CorrelationId, new[] { StepContext.Instance[General.CorrelationId] });

        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Request to {request.RequestUri} returned unsuccessful status code {response.StatusCode}, correlationId: {correlationId}");
                Console.WriteLine($"{await response.Content.ReadAsStringAsync(cancellationToken)}");
            }

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send request to {request.RequestUri}, correlationId: {correlationId}");
            throw;
        }
    }
}

internal class LogDelegatingHandlerFactory
{
    public LogDelegatingHandler Create() =>
        new();
}
