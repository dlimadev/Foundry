using Foundry.Domain.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Sample.FinancialMarket.Infrastructure.Providers
{
    /// <summary>
    /// An implementation of ICurrentUserService that retrieves the current user's ID
    /// from the HttpContext provided by ASP.NET Core.
    /// This implementation is specific to a web application context.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HttpContext accessor to get user claims.</param>
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the user ID from the 'NameIdentifier' claim of the authenticated principal.
        /// Returns null if the user is not authenticated or the claim is not present.
        /// </summary>
        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}