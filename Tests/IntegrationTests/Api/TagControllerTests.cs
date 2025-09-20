using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace IntegrationTests.Api;

public class TagControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public TagControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private record RefreshResponse(string status);

    private async Task ResetDbAsync(Action<AppDbContext>? seed = null)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        seed?.Invoke(db);
        await db.SaveChangesAsync();
    }

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

    [Fact]
    public async Task GetTags_ShouldReturnFirstPageSortedByCountDesc()
    {
        await ResetDbAsync(db =>
        {
            db.Tags.AddRange(
                new Tag { Name = "csharp", Count = 50 },
                new Tag { Name = "java", Count = 30 },
                new Tag { Name = "python", Count = 70 }
            );
        });

        var response = await _client.GetAsync("/api/tag?page=1&size=2&sortBy=count&order=desc");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var data = json.GetProperty("data").EnumerateArray().ToList();

        data.Should().HaveCount(2);
        data[0].GetProperty("name").GetString().Should().Be("python");
        data[1].GetProperty("name").GetString().Should().Be("csharp");
    }

    [Fact]
    public async Task GetTags_ShouldReturnSecondPageWithRemainingTag()
    {
        await ResetDbAsync(db =>
        {
            db.Tags.AddRange(
                new Tag { Name = "csharp", Count = 50 },
                new Tag { Name = "java", Count = 30 },
                new Tag { Name = "python", Count = 70 }
            );
        });

        var response = await _client.GetAsync("/api/tag?page=2&size=2&sortBy=count&order=desc");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var data = json.GetProperty("data").EnumerateArray().ToList();

        data.Should().HaveCount(1);
        data[0].GetProperty("name").GetString().Should().Be("java");
    }
}
