using CatScraper.Application.Features.Images.Queries.GetImage;
using CatScraper.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatScraper.WebApi.Endpoints.Images;

public static class ImageEndpoints
{
    public static RouteGroupBuilder MapImageEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("get/{id}", async (int id, IMediator mediator, bool download = false) =>
        {
            if (id <= 0)
                return Results.BadRequest(new ApiResponseResult(true,
                    [Error.Validation(description: "Id should be 1 and above.")]));
            
            var response = await mediator.Send(new GetImageRequest()
            {
                Id = id
            });

            if (response.IsError)
            {
                return Results.BadRequest(new ApiResponseResult(response.IsError, response.Errors));
            }

            var contentType = $"image/{response.Value.Image.ImageExtension.TrimStart('.')}";
            if (!download)
            {
                return Results.File(response.Value.Image.ImageData, contentType);
            } 
            
            var fileName = $"{id}{response.Value.Image.ImageExtension}";
            return Results.File(response.Value.Image.ImageData, contentType, fileName);
        });
        return group;
    }
}