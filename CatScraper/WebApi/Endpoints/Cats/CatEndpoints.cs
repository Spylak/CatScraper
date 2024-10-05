using CatScraper.Application.Features.Cats.Commands.FetchCats;
using CatScraper.Application.Features.Cats.Queries.GetCat;
using CatScraper.Application.Features.Cats.Queries.GetCats;
using CatScraper.WebApi.Common;
using CatScraper.WebApi.Extensions;
using MediatR;

namespace CatScraper.WebApi.Endpoints.Cats;

public static class CatEndpoints
{
    public static RouteGroupBuilder MapCatEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("fetch", async (HttpRequest request, IMediator mediator) =>
        {
            var apiKey = request.GetApiKey();
            var response = await mediator.Send(new FetchCatsRequest()
            {
                ApiKey = apiKey,
                BaseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}"
            });

            if (response.IsError)
            {
                return Results.BadRequest(new ApiResponseResult(response.IsError, response.Errors));
            }

            if (response.Value.Count < 25)
            {
                return Results.Ok(new ApiResponseResult(response.IsError, response.Value,
                [
                    Error.Conflict(description: "You have reached the last page of images available for download.")
                ]));
            }

            return Results.Ok(new ApiResponseResult(response.IsError, response.Value));
        });

        group.MapGet("{id}", async (int id, HttpRequest request, IMediator mediator) =>
        {
            if (id <= 0)
                return Results.BadRequest(new ApiResponseResult(true,
                    [Error.Validation(description: "Id should be 1 and above.")]));

            var response = await mediator.Send(new GetCatRequest()
            {
                Id = id,
                BaseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}"
            });

            return response.IsError ? Results.BadRequest(new ApiResponseResult(response.IsError, response.Errors)) : Results.Ok(new ApiResponseResult(response.IsError, response.Value));
        });

        group.MapGet("",
            async (IMediator mediator, HttpRequest request, int page = 1, int pageSize = 10, string? tag = null) =>
            {
                if (page <= 0)
                    return Results.BadRequest(new ApiResponseResult(true,
                        [Error.Validation(description: "Page should be 1 and above.")]));

                if (pageSize < 0)
                    return Results.BadRequest(new ApiResponseResult(true,
                        [Error.Validation(description: "Page size should be 0 and above.")]));
                
                if (tag?.Length > 50)
                    return Results.BadRequest(new ApiResponseResult(true,
                        [Error.Validation(description: "Tag characters cannot exceed 50.")]));
                
                var response = await mediator.Send(new GetCatsRequest()
                {
                    PageSize = pageSize,
                    Page = page,
                    Tag = tag,
                    BaseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}"
                });
                
                return response.IsError ? Results.BadRequest(new ApiResponseResult(response.IsError, response.Errors)) : Results.Ok(new ApiResponseResult(response.IsError, response.Value));
            });

        return group;
    }
}