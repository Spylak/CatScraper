using CatScraper.Application.Abstractions;
using CatScraper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatScraper.Infrastructure;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddInfrastructureLayer(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        AddPersistence(builder);
        return builder;
    }
    
    private static void AddPersistence(WebApplicationBuilder builder)
    {
        var connectionString =
            builder.Configuration["ConnectionString"] ??
            throw new ArgumentNullException(nameof(builder.Configuration));

        builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlServer(connectionString); });

        builder.Services.AddScoped<IAppDbContext, AppDbContext>();
    }
    
    public static async Task ExecuteInfrastructureOnStartup(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if ((await db.Database.GetPendingMigrationsAsync()).Any())
        {
            await db.Database.MigrateAsync();
        }
    }
}