using Foundry.Application.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Foundry.Api.BuildingBlocks.Middlewares
{
    /// <summary>
    /// A middleware that reads metadata from the IResponseMetaProvider and adds it
    /// to the HTTP response headers as custom 'X-' headers.
    /// </summary>
    public class ResponseHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IResponseMetaProvider metaProvider)
        {
            // Register a callback that will be executed just before the response headers are sent.
            context.Response.OnStarting(() => {

                var metadata = metaProvider.GetMetadata();
                foreach (var item in metadata)
                {
                    // Add each piece of metadata as a custom 'X-' header.
                    // e.g., "DataSource" becomes "X-DataSource"
                    context.Response.Headers.Append($"X-{item.Key}", item.Value.ToString());
                }

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

    public static class ResponseHeaderMiddlewareExtensions
    {
        /// <summary>
        /// Adds the ResponseHeaderMiddleware to the application's request pipeline.
        /// </summary>
        public static IApplicationBuilder UseFoundryResponseHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseHeaderMiddleware>();
        }
    }
}