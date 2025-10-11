using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Foundry.Api.BuildingBlocks.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring Swagger/OpenAPI generation.
    /// </summary>
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddFoundrySwagger(this IServiceCollection services, string apiTitle)
        {
            services.AddSwaggerGen(options =>
            {
                // This allows Swagger to generate a document for each discovered API version.
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = $"{apiTitle} {description.ApiVersion}",
                        Version = description.ApiVersion.ToString()
                    });
                }

                // Add JWT Authentication support to the Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            return services;
        }
    }
}