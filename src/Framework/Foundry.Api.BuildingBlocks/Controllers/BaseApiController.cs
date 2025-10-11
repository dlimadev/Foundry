// The code should be in English
using Foundry.Application.Abstractions.Responses;
using Foundry.Api.BuildingBlocks.ApiContracts.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;

namespace Foundry.Api.BuildingBlocks.Controllers
{
    /// <summary>
    /// An abstract base controller that provides common functionalities, such as handling
    /// the standard 'Result<T>' object and converting it to an appropriate IActionResult.
    /// All API controllers in the consuming application should inherit from this class.
    /// </summary>
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Handles the result from an application service, converting it into a standard,
        /// JSON:API compliant HTTP response (IActionResult).
        /// </summary>
        /// <typeparam name="T">The type of the data in the result.</typeparam>
        /// <param name="result">The Result<T> object from the service layer.</param>
        /// <returns>An IActionResult (e.g., Ok, BadRequest, NotFound).</returns>
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess && result.Value != null)
            {
                // --- Handle Success Case ---
                var envelope = JsonApiEnvelope<T>.Success(result.Value);
                var statusCode = result.SuggestedStatusCode ?? GetDefaultSuccessStatusCode();

                return new ObjectResult(envelope) { StatusCode = (int)statusCode };
            }
            else
            {
                // --- Handle Failure Case ---
                var errorStatusCode = result.SuggestedStatusCode ?? GetDefaultErrorStatusCode(result.Notifications);
                var apiErrors = result.Notifications
                    .Select(n => new JsonApiError(((int)errorStatusCode).ToString(), n.Key, n.Message));
                var errorEnvelope = JsonApiEnvelope<object>.Error(apiErrors);

                return new ObjectResult(errorEnvelope) { StatusCode = (int)errorStatusCode };
            }
        }

        private HttpStatusCode GetDefaultSuccessStatusCode() => HttpContext.Request.Method switch
        {
            "POST" => HttpStatusCode.Created,
            "DELETE" => HttpStatusCode.NoContent,
            _ => HttpStatusCode.OK
        };

        private HttpStatusCode GetDefaultErrorStatusCode(IReadOnlyCollection<Foundry.Domain.Notifications.Notification> notifications)
        {
            if (notifications.Any(n => n.Key.EndsWith(".notFound"))) return HttpStatusCode.NotFound;
            if (notifications.Any(n => n.Key.EndsWith(".conflict"))) return HttpStatusCode.Conflict;
            return HttpStatusCode.BadRequest;
        }
    }
}