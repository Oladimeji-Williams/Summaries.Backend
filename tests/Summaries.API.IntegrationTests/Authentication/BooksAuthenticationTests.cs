using System.Net;
using System.Net.Http.Headers;
using Summaries.API.IntegrationTests.Fixtures;

namespace Summaries.API.IntegrationTests.Authentication;

public sealed class BooksAuthenticationTests(
    CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAllBooks_WithoutAuthentication_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.AuthenticatedHeader, "false");

        var response = await _client.GetAsync("/api/v1/books");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllBooks_WithAuthentication_ReturnsOk()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.SchemeName);

        var response = await _client.GetAsync("/api/v1/books");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}