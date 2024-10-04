using CatScraper.Application;
using CatScraper.Infrastructure;
using CatScraper.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructureLayer()
    .AddApplicationLayer()
    .AddWebApiLayer();

var app = builder.Build();

app.AddWebApiLayer();
await app.ExecuteInfrastructureOnStartup();
app.Run();

//To convert Program into public instead of internal so that the test project can pick it up.
public partial class Program
{
}