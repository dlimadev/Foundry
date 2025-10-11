// The code should be in English
using FluentValidation;
using Foundry.Domain.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace Foundry.Api.BuildingBlocks.Middlewares
{
    /// <summary>
    /// Provides an extension method to add a global exception handling middleware to the application pipeline.
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        /// <summary>
        /// Adds a global exception handling middleware that catches exceptions and maps them
        /// to a standardized RFC 7807 ProblemDetails response.
        /// </summary>
        /// <param name="app">The IApplicationBuilder instance.</param>
        /// <param name="environment">The host environment to determine if stack traces should be included.</param>
        public static void UseFoundryExceptionHandler(this IApplicationBuilder app, IHostEnvironment environment)
        {
            _ = app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    // --- CORRECTION ---
                    // Instead of ILogger<Program>, we resolve the ILoggerFactory and create a logger
                    // with a specific category. This makes the middleware self-contained and reusable.
                    var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("Foundry.ExceptionHandler");

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        context.Response.ContentType = "application/problem+json";

                        var error = contextFeature.Error;
                        var title = "An unexpected error occurred.";
                        var detail = error.Message;

                        // Default to 500 Internal Server Error
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        // Customize response based on specific, known exception types
                        switch (error)
                        {
                            case DomainException domainException:
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                title = "Business Rule Violation";
                                // The detail can be mapped to a user-friendly message using I18N and the ErrorCode in the API layer.
                                // For now, we provide a developer-friendly message.
                                detail = $"Error Code: {domainException.ErrorCode}. Parameters: [{string.Join(", ", domainException.Parameters)}]";
                                break;

                            case ValidationException validationException:
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                title = "Validation Error";
                                detail = "One or more validation errors occurred.";
                                // In a real app, you could serialize validationException.Errors here into an 'errors' field.
                                break;

                            case KeyNotFoundException keyNotFoundException:
                                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                title = "Resource Not Found";
                                detail = keyNotFoundException.Message;
                                break;
                        }

                        if (context.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
                        {
                            logger.LogError(error, "An unhandled exception occurred: {Message}", error.Message);
                        }
                        else
                        {
                            logger.LogWarning("A handled exception occurred: {ExceptionType} - {Message}", error.GetType().Name, error.Message);
                        }

                        var problemDetails = new
                        {
                            Status = context.Response.StatusCode,
                            Title = title,
                            Detail = detail,
                            Trace = environment.IsDevelopment() ? error.StackTrace : null
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                    }
                });
            });
        }
    }
}