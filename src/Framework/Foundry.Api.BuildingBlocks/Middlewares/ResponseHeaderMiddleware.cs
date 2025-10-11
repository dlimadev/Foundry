using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Foundry.Api.BuildingBlocks.Middlewares
{
    public class ResponseHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // The middleware no longer needs to inject IResponseMetaProvider.
        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() => {

                // Read the metadata directly from the HttpContext.Items dictionary.
                if (context.Items.TryGetValue("DataSource", out var dataSource))
                {
                    if (dataSource != null)
                    {
                        context.Response.Headers.Append("X-DataSource", dataSource.ToString());
                    }
                }

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

    public static class ResponseHeaderMiddlewareExtensions
    {
        public static IApplicationBuilder UseFoundryResponseHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseHeaderMiddleware>();
        }
    }
}