namespace CatScraper.WebApi.Extensions;

public static class RequestExtensions
{
    public static string? GetApiKey(this HttpRequest request)
    {
        if (request.Headers.TryGetValue("x-api-key", out var headerApiKey))
        {
            return headerApiKey;
        }

        if (request.Query.TryGetValue("api_key", out var queryApiKey))
        {
            return queryApiKey;
        }

        return null;
    }
}