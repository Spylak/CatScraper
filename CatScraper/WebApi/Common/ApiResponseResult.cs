namespace CatScraper.WebApi.Common;

public record ApiResponseResult
{
    public ApiResponseResult()
    {
        IsError = true;
        Messages = [];
    }
    
    public ApiResponseResult(bool? isError = null)
    {
        IsError = isError ?? true;
        Messages = [];
        Data = null;
    }
    
    public ApiResponseResult(bool? isError = null, Dictionary<string, string>? messages = null)
    {
        IsError = isError ?? true;
        Messages = messages ?? [];
        Data = null;
    }
    
    public ApiResponseResult(bool? isError = null, object? data = null)
    {
        IsError = isError ?? true;
        Messages = [];
        Data = data;
    }
    
    public ApiResponseResult(bool? isError = null, object? data = null, Dictionary<string, string>? messages = null)
    {
        IsError = isError ?? true;
        Messages = messages ?? [];
        Data = data;
    }
    
    public ApiResponseResult(bool? isError = null, object? data = null, string? message = null)
    {
        IsError = isError ?? true;
        if (message is not null)
            Messages["Generic"] = message; 
        Data = data;
    }
    
    public ApiResponseResult(bool isError, List<Error> errors)
    {
        IsError = isError;
        Messages = errors.ToDictionary(i=>i.Code, i => i.Description);
    }
    
    public ApiResponseResult(bool isError, object? data, List<Error> errors)
    {
        IsError = isError;
        Messages = errors.ToDictionary(i=>i.Code, i => i.Description);
        Data = data;
    }

    public bool IsError { get; set; }
    public Dictionary<string, string> Messages { get; set; } = new();

    public object? Data { get; init; }
}

public record ApiResponseResult<T> : ApiResponseResult where T : class?
{
    public ApiResponseResult()
    {
        IsError = true;
        Messages = [];
    }

    public ApiResponseResult(T? data = null, bool? isError = null, Dictionary<string, string>? messages = null) : base(isError, messages)
    {
        Data = data;
    }

    public new T? Data { get; set; }
}