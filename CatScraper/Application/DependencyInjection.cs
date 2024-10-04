using CatScraper.Application.Helpers;
using CatScraper.Domain.Options;

namespace CatScraper.Application;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApplicationLayer(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<CatsApiOption>(builder.Configuration.GetSection("CatsApi"));
        builder.Services.AddSingleton<CatFetchLockHelper>();
        return builder;
    }
}