using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace ticket.api.Middleware;

public static class PollyMiddleware
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        var policy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(message => message.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        return policy;
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        var policy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        return policy;
    }
}
