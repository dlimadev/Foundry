using Polly;
using Polly.Extensions.Http;

namespace Foundry.Infrastructure.Resilience
{
    /// <summary>
    /// Provides a set of standard, reusable resilience policies using Polly.
    /// </summary>
    public static class ResiliencePolicies
    {
        /// <summary>
        /// Gets a standard HTTP retry policy that retries 3 times with an exponential backoff for transient failures.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        /// <summary>
        /// Gets a standard HTTP circuit breaker policy that breaks the circuit for 30 seconds after 5 consecutive failures.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}