using System.Net;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using CatScraper.Application.Features.Cats.Queries.GetCats;
using CatScraper.WebApi.Common;

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
        var tasks = new List<Task<HttpResponseMessage>>();
        httpClient.DefaultRequestHeaders.Add("x-api-key",_catApiKey);
        for (var i = 0; i < 4; i++)
        {
            tasks.Add(httpClient.PostAsync("api/cats/fetch", new StringContent("")));
        }
        
        await Task.WhenAll(tasks);
        if (tasks.All(t => t.Result.StatusCode != HttpStatusCode.OK))
        {
            var strBlder = new StringBuilder();
            strBlder.Append("Couldn't populate the database");
            strBlder.Append('\n');
            foreach (var task in tasks)
            {
                var content = await task.Result.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseResult>(content, _jsonSerializerSettings);

                foreach (var entry in apiResponse?.Messages ?? [])
                {
                    strBlder.Append(entry.Key);
                    strBlder.Append('=');
                    strBlder.Append(entry.Value);
                    strBlder.Append('\n');
                }
            }
            
            throw new Exception(strBlder.ToString());
        }

        _webApplicationFactory.PopulateDatabaseInvoked = true;
    }
    
    [Fact]
    public async Task GivenPostRequestToCatsEndpoint_ShouldReturnOkay()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.PopulateDatabaseInvoked)
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
        if (!_webApplicationFactory.PopulateDatabaseInvoked)
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
        if (!_webApplicationFactory.PopulateDatabaseInvoked)
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
        if (!_webApplicationFactory.PopulateDatabaseInvoked)
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
        if (!_webApplicationFactory.PopulateDatabaseInvoked)
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
        if (!_webApplicationFactory.PopulateDatabaseInvoked)
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
        if (!_webApplicationFactory.PopulateDatabaseInvoked)
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
    
    [Fact]
    public async Task GivenGetRequestToCatsEndpoint_ShouldReturnPaginatedResultsWithoutDuplicates()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        if (!_webApplicationFactory.PopulateDatabaseInvoked)
        {
            await PopulateDatabase(httpClient);
        }

        const int pageSize = 15;
        const int totalPages = 4;
        var allCats = new List<GetCatsResponse>();

        for (int page = 1; page <= totalPages; page++)
        {
            var uri = $"api/cats?page={page}&pageSize={pageSize}&tag=Active";
            var response = await httpClient.GetAsync(uri);
        
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var catList = JsonConvert.DeserializeObject<ApiResponseResult<List<GetCatsResponse>>>(content, _jsonSerializerSettings);
        
            catList?.Data.Should().NotBeNull();
            catList!.Data!.Count.Should().BeLessOrEqualTo(pageSize);

            allCats.AddRange(catList.Data);
        }

        allCats.Count.Should().BeGreaterThan(0);
        allCats.Count.Should().BeLessOrEqualTo(totalPages * pageSize);

        var distinctCats = allCats.Distinct(new CatEqualityComparer());
        distinctCats.Count().Should().Be(allCats.Count);

        var orderedCats = allCats.OrderBy(c => c.Id).ToList();
        allCats.Should().BeEquivalentTo(orderedCats, options => options.WithStrictOrdering());
    }

    private class CatEqualityComparer : IEqualityComparer<GetCatsResponse>
    {
        public bool Equals(GetCatsResponse? x, GetCatsResponse? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(GetCatsResponse obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}