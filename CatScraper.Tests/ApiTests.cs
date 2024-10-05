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
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore
    };
    private readonly string _catApiKey = "";

    public ApiTests(CustomWebApplicationFactory webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    private async Task PopulateDatabase(HttpClient httpClient)
    {
        var tasks = new List<Task>();
        httpClient.DefaultRequestHeaders.Add("x-api-key",_catApiKey);
        for (int i = 0; i < 4; i++)
        {
            tasks.Add(httpClient.PostAsync("api/cats/fetch", new StringContent("")));
        }
        await Task.WhenAll(tasks);
        _webApplicationFactory.IsDatabasePopulated = tasks.Any(i => i.IsCompletedSuccessfully);
    }
    
    [Fact]
    public async Task GivenPostRequestToCatsEndpoint_ShouldReturnOkay()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.IsDatabasePopulated)
        {
            await PopulateDatabase(httpClient);
        }
        httpClient.DefaultRequestHeaders.Add("x-api-key", _catApiKey);
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
        strBldr.Append(100);
        strBldr.Append('&');
        strBldr.Append("tag=");
        strBldr.Append("Active");
        var uri = strBldr.ToString();
        var response = await httpClient.GetAsync(uri);
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var catList = JsonConvert.DeserializeObject<ApiResponseResult<List<GetCatsResponse>>>(content, _jsonSerializerSettings);
        catList?.Data.Should().NotBeNull();
        catList!.Data!.Count.Should().BeGreaterThan(0);
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
        var catList = JsonConvert.DeserializeObject<ApiResponseResult<List<GetCatsResponse>>>(content, _jsonSerializerSettings);
        catList?.Data.Should().NotBeNull();
        catList!.Data!.Count.Should().Be(0);
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
        var catList = JsonConvert.DeserializeObject<ApiResponseResult<List<GetCatsResponse>>>(content, _jsonSerializerSettings);
        catList?.Data.Should().NotBeNull();
        catList!.Data!.Count.Should().BeGreaterThan(0);
        catList.Data!.Count.Should().BeLessOrEqualTo(25);
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
        var catList = JsonConvert.DeserializeObject<ApiResponseResult<List<GetCatsResponse>>>(content, _jsonSerializerSettings);
        catList?.Data.Should().NotBeNull();
        catList!.Data!.Count.Should().BeGreaterThan(0);
        catList.Data!.Count.Should().BeLessOrEqualTo(10);
    }
}