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
                return Results.BadRequest(new ApiResponse(false, response.Errors));
            }

            if (response.Value.Count < 25)
            {
                return Results.Ok(new ApiResponse(true,
                [
                    Error.Conflict(description: "You have reached the last page of images available for download.")
                ], response.Value));
            }

            return Results.Ok(new ApiResponse(true, response.Value));
        });

        group.MapGet("{id}", async (int id, HttpRequest request, IMediator mediator) =>
        {
            var response = await mediator.Send(new GetCatRequest()
            {
                Id = id,
                BaseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}"
            });

            if (response.FirstError.Code == "General.NotFound")
            {
                return Results.NotFound(new ApiResponse(false, response.Errors));
            }

            return Results.Ok(new ApiResponse(true, response.Value));
        });

        group.MapGet("",
            async (IMediator mediator, HttpRequest request, int page = 1, int pageSize = 10, string? tag = null) =>
            {
                if (page <= 0)
                    return Results.BadRequest(new ApiResponse(false,
                        [Error.Validation(description: "Page should be 1 and above.")]));

                if (pageSize < 0)
                    return Results.BadRequest(new ApiResponse(false,
                        [Error.Validation(description: "Page size should be 0 and above.")]));

                var response = await mediator.Send(new GetCatsRequest()
                {
                    PageSize = pageSize,
                    Page = page,
                    Tag = tag,
                    BaseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}"
                });

                return Results.Ok(new ApiResponse(true, response.Value));
            });

        return group;
    }
}