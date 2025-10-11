using Foundry.Domain.Interfaces.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Foundry.Infrastructure.Http
{
    /// <summary>
    /// Concrete implementation of an abstracted, resilient, and authenticated HTTP client.
    /// </summary>
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthHeaderProvider? _authHeaderProvider;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientService"/> class.
        /// </summary>
        public HttpClientService(IHttpClientFactory httpClientFactory, IAuthHeaderProvider? authHeaderProvider = null)
        {
            _httpClientFactory = httpClientFactory;
            _authHeaderProvider = authHeaderProvider;
            _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        
        /// <inheritdoc />
        public async Task<T?> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
        {
            var httpClient = await CreateClientWithAuthAsync();
            var response = await httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task PostAsync<TRequest>(string uri, TRequest data, CancellationToken cancellationToken = default)
        {
            var httpClient = await CreateClientWithAuthAsync();
            var response = await httpClient.PostAsJsonAsync(uri, data, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        
        /// <inheritdoc />
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string uri, TRequest data, CancellationToken cancellationToken = default)
        {
            var httpClient = await CreateClientWithAuthAsync();
            var response = await httpClient.PostAsJsonAsync(uri, data, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonSerializerOptions, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Creates an HttpClient instance from the factory and applies authentication if a provider is available.
        /// </summary>
        private async Task<HttpClient> CreateClientWithAuthAsync()
        {
            var client = _httpClientFactory.CreateClient("FoundryApiClient");
            if (_authHeaderProvider != null)
            {
                await _authHeaderProvider.SetAuthHeader(client);
            }
            return client;
        }
    }
}