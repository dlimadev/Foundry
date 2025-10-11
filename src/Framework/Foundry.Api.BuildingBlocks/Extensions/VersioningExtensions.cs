using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Foundry.Api.BuildingBlocks.Extensions
{
    /// <summary>
    // Provides an extension method to configure API versioning in a standardized way.
    /// </summary>
    public static class VersioningExtensions
    {
        public static IServiceCollection AddFoundryApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                // Reports the supported API versions in the 'api-supported-versions' response header.
                options.ReportApiVersions = true;

                // Sets the default API version to 1.0 if a client doesn't specify one.
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Configures how the version is read from the request. We use the URL segment.
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                // Formats the version in the Swagger UI dropdown (e.g., 'v1', 'v2').
                options.GroupNameFormat = "'v'VVV";

                // Substitutes the version placeholder in route templates.
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}