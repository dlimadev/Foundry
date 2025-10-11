namespace Foundry.Domain.Interfaces.Http
{
    /// <summary>
    /// Defines a contract for an abstracted, resilient, and authenticated HTTP client.
    /// This interface lives in the Domain layer to be used by the Application layer,
    /// while its implementation resides in the Infrastructure layer.
    /// </summary>
    public interface IHttpClientService
    {
        /// <summary>
        /// Performs a GET request and deserializes the response to the specified type.
        /// </summary>
        Task<T?> GetAsync<T>(string uri, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a POST request with the given data.
        /// </summary>
        Task PostAsync<TRequest>(string uri, TRequest data, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Performs a POST request with the given data and deserializes the response.
        /// </summary>
        Task<TResponse?> PostAsync<TRequest, TResponse>(string uri, TRequest data, CancellationToken cancellationToken = default);
    }
}