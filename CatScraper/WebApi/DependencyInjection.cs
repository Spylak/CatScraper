using CatScraper.WebApi.Endpoints.Cats;
using CatScraper.WebApi.Endpoints.Images;
using CatScraper.WebApi.Extensions;
using Microsoft.OpenApi.Models;

namespace CatScraper.WebApi;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddWebApiLayer(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatScraper API", Version = "v1" });

            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "x-api-key",
                Type = SecuritySchemeType.ApiKey,
                Description = "API Key needed to access endpoints."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", b =>
            {
                b.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        return builder;
    }

    public static WebApplication AddWebApiLayer(this WebApplication app)
    {
        app.UseCors("AllowAll");
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapGroup("/api/cats")
            .MapCatEndpoints()
            .WithTags("Cat Endpoints");

        app.MapGroup("/api/images")
            .MapImageEndpoints()
            .WithTags("Image Endpoints");
        
        app.Use(async (context, next) =>
        {
            await next();
        
            // Check if the status code is 404
            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                context.Response.Redirect("/swagger/index.html");
            }
        });

        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        app.UseDefaultExceptionHandler(logger);
        
        return app;
    }
}