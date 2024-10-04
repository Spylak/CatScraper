namespace CatScraper.Domain.Extensions;

public static class StringExtensions
{
    public static string ToPascalCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return str;

        return char.ToUpperInvariant(str[0]) + str[1..].ToLowerInvariant();
    }
}