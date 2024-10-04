using CatScraper.Application.Features.Images.Queries.GetImage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatScraper.WebApi.Endpoints.Images;

public static class ImageEndpoints
{
    public static RouteGroupBuilder MapImageEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("get/{id}", async (int id, IMediator mediator, bool download = false) =>
        {
            var response = await mediator.Send(new GetImageRequest()
            {
                Id = id
            });

            if (response.IsError)
            {
                return Results.NotFound(response.FirstError);
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