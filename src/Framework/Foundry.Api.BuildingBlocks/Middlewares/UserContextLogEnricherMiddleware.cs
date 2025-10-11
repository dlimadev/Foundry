// The code should be in English
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Security.Claims;

namespace Foundry.Api.BuildingBlocks.Middlewares
{
    /// <summary>
    /// A middleware that enriches the logging context with user information from the current authenticated principal.
    /// </summary>
    public class UserContextLogEnricherMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextLogEnricherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get the user identity from the HttpContext
            var user = context.User;
            string userId = "Anonymous";

            if (user?.Identity?.IsAuthenticated == true)
            {
                // Get the user ID from the 'sub' or 'nameidentifier' claim, which is standard in JWT/OIDC.
                userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "AuthenticatedUser_NoIdClaim";
            }

            // Push the UserId to the Serilog LogContext. All logs generated
            // during this request will automatically have this property.
            using (LogContext.PushProperty("UserId", userId))
            {
                await _next(context);
            }
        }
    }

    public static class UserContextLogEnricherMiddlewareExtensions
    {
        public static IApplicationBuilder UseFoundryUserContextLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserContextLogEnricherMiddleware>();
        }
    }
}