using System.Net;
using CatScraper.WebApi.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace CatScraper.WebApi.Extensions;

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseDefaultExceptionHandler(this IApplicationBuilder app,
        ILogger logger,
        string genericErrorMessage = "Something went wrong.",
        bool logStructuredException = false,
        bool useGenericReason = false
    )
    {
        app.UseExceptionHandler(
            errApp =>
            {
                errApp.Run(
                    async ctx =>
                    {
                        var exHandlerFeature = ctx.Features.Get<IExceptionHandlerFeature>();

                        if (exHandlerFeature is not null)
                        {
                            var route = exHandlerFeature.Endpoint?.DisplayName?.Split(" => ")[0];
                            var exceptionType = exHandlerFeature.Error.GetType().Name;
                            var reason = exHandlerFeature.Error.Message;

                            if (logStructuredException)
                                logger.LogError(exHandlerFeature.Error,
                                    "[{@exceptionType}] at [{@route}] due to [{@reason}]", exceptionType, route,
                                    reason);
                            else
                            {
                                //this branch is only meant for unstructured textual logging
                                logger.LogError(
                                    $"""
                                     =================================
                                     {route}
                                     TYPE: {exceptionType}
                                     REASON: {reason}
                                     ---------------------------------
                                     {exHandlerFeature.Error.StackTrace}
                                     """);
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            ctx.Response.ContentType = "application/problem+json";
                            await ctx.Response.WriteAsJsonAsync(
                                new ApiResponseResult()
                                {
                                    Data = "See application log for stack trace.",
                                    IsError = true,
                                    Messages = {{"Main", useGenericReason ? genericErrorMessage : reason}}
                                });
                        }
                    });
            });

        return app;
    }
}