// The code should be in English
using System.Net.Http;
using System.Threading.Tasks;

namespace Foundry.Domain.Interfaces.Http
{
    /// <summary>
    /// Defines a contract for a provider that can supply an authentication header
    /// to an HttpClient instance before a request is made. This decouples the
    /// HttpClientService from the specific authentication mechanism.
    /// </summary>
    public interface IAuthHeaderProvider
    {
        /// <summary>
        /// Sets the appropriate authentication header on the provided HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to be configured.</param>
        Task SetAuthHeader(HttpClient httpClient);
    }
}