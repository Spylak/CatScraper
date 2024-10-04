using System.Net;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using CatScraper.Application.Features.Cats.Queries.GetCats;
using CatScraper.WebApi.Common;
using Newtonsoft.Json.Linq;

namespace CatScraper.Tests;

public class ApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _webApplicationFactory;

    public ApiTests(CustomWebApplicationFactory webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    private async Task PopulateDatabase(HttpClient httpClient)
    {
        var successfulTasks = new List<bool>();
        httpClient.DefaultRequestHeaders.Add("x-api-key",_webApplicationFactory.CatApiKey);
        for (int i = 0; i < 4; i++)
        {
            var response = await httpClient.PostAsync("api/cats/fetch", new StringContent(""));
            if (response.IsSuccessStatusCode)
            {
                successfulTasks.Add(true);
            }
            await Task.Delay(500);
        }
        _webApplicationFactory.IsDatabasePopulated = successfulTasks.Any(i=>i);
    }
    
    [Fact]
    public async Task GivenPostRequestToCatsEndpoint_ShouldReturnOkay()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.IsDatabasePopulated)
        {
            await PopulateDatabase(httpClient);
        }
        httpClient.DefaultRequestHeaders.Add("x-api-key",_webApplicationFactory.CatApiKey);
        var response = await httpClient.PostAsync("api/cats/fetch", new StringContent(""));
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }
    
    //This is hit or miss depending if the tag = Calm got into the first pages of cats.
    [Fact]
    public async Task GivenGetRequestToCatsEndpoint_ShouldReturnOkay()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.IsDatabasePopulated)
        {
            await PopulateDatabase(httpClient);
        }
        var strBldr = new StringBuilder();
        strBldr.Append("api/cats");
        strBldr.Append('?');
        strBldr.Append("page=");
        strBldr.Append(1);
        strBldr.Append('&');
        strBldr.Append("pageSize=");
        strBldr.Append(25);
        strBldr.Append('&');
        strBldr.Append("tag=");
        strBldr.Append("Calm");
        var uri = strBldr.ToString();
        var response = await httpClient.GetAsync(uri);
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse>(content);
        var catList = ((JArray?)result?.Data)?.ToObject<List<GetCatsResponse>>();
        catList.Should().NotBeNull();
        catList!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GivenPageZero_ShouldReturnBadRequest()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.IsDatabasePopulated)
        {
            await PopulateDatabase(httpClient);
        }
        var response = await httpClient.GetAsync("api/cats?page=0&pageSize=25");
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenInvalidPageSize_ShouldReturnBadRequest()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.IsDatabasePopulated)
        {
            await PopulateDatabase(httpClient);
        }
        var response = await httpClient.GetAsync("api/cats?page=1&pageSize=-1");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenNonexistentTag_ShouldReturnEmptyList()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.IsDatabasePopulated)
        {
            await PopulateDatabase(httpClient);
        }
        var response = await httpClient.GetAsync("api/cats?page=1&pageSize=25&tag=NonexistentTag1234567890");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse>(content);
        var catList = ((JArray?)result?.Data)?.ToObject<List<GetCatsResponse>>();
        catList.Should().NotBeNull();
        catList!.Count.Should().Be(0);
    }

    [Fact]
    public async Task GivenMissingPageQueryParam_ShouldReturnPageSize()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.IsDatabasePopulated)
        {
            await PopulateDatabase(httpClient);
        }
        var response = await httpClient.GetAsync("api/cats?pageSize=25");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse>(content);
        var catList = ((JArray?)result?.Data)?.ToObject<List<GetCatsResponse>>();
        catList.Should().NotBeNull();
        catList!.Count.Should().BeGreaterThan(0);
        catList.Count.Should().BeLessOrEqualTo(25);
    }

    [Fact]
    public async Task GivenMissingPageSizeQueryParam_ShouldReturnDefaultPageSize()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.IsDatabasePopulated)
        {
            await PopulateDatabase(httpClient);
        }
        var response = await httpClient.GetAsync("api/cats?page=1");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse>(content);
        var catList = ((JArray?)result?.Data)?.ToObject<List<GetCatsResponse>>();
        catList.Should().NotBeNull();
        catList!.Count.Should().BeGreaterThan(0);
        catList.Count.Should().BeLessOrEqualTo(10);
    }
}