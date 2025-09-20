using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace IntegrationTests.Api;

public class TagControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TagControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private record RefreshResponse(string status);

    [Fact]
    public async Task GetTags_ShouldReturnOkAndData()
    {
        var response = await _client.GetAsync("/api/tag?page=1&size=10&sortBy=name&order=asc");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        result.TryGetProperty("data", out var data).Should().BeTrue();
        data.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task RefreshTags_ShouldReturnOk()
    {
        var response = await _client.PostAsync("/api/tag/refresh", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<RefreshResponse>();

        json.Should().NotBeNull();
        json!.status.Should().Be("refresh started");
    }
}
