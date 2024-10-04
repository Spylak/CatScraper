namespace CatScraper.WebApi.Common;

public class ApiResponse
{
    public ApiResponse()
    {
        
    }

    public ApiResponse(bool isSuccess, object? data)
    {
        IsSuccess = isSuccess;
        Data = data;
    }

    public ApiResponse(bool isSuccess, List<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors.ToDictionary(i=>i.Code, i => i.Description);
    }
    
    public ApiResponse(bool isSuccess, List<Error> errors, object? data)
    {
        IsSuccess = isSuccess;
        Errors = errors.ToDictionary(i=>i.Code, i => i.Description);
        Data = data;
    }
    public bool IsSuccess { get; set; }
    public virtual object? Data { get; set; }
    public Dictionary<string, string> Errors { get; set; } = [];
}