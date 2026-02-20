using Microsoft.AspNetCore.Builder;
using Shared.Utilities.Middleware;

namespace Shared.Utilities.Extensions;

/// <summary>
/// Extension methods for configuring the application pipeline.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the global exception handling middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
