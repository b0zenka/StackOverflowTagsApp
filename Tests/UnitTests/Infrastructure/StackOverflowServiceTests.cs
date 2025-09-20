using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;

namespace UnitTests.Infrastructure;

public class StackOverflowServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private HttpClient GetFakeHttpClient(string json)
    {
        var handler = new FakeHttpMessageHandler(json);
        return new HttpClient(handler);
    }

    [Fact]
    public async Task FetchTagsAsync_ShouldInsertTags()
    {
        // Arrange
        var json = @"{
            ""items"": [{ ""name"": ""tag_test"", ""count"": 123 }],
            ""has_more"": false
        }";

        using var db = GetDbContext();
        var client = GetFakeHttpClient(json);

        // Act
        var service = new StackOverflowService(db, client);
        var fetched = await service.FetchTagsAsync(1);

        // Assert
        Assert.Equal(1, fetched);
        Assert.Single(db.Tags);
    }

    [Fact]
    public async Task FetchTagsAsync_ShouldUpdateExistingTag()
    {
        // Arrange
        var json = @"{
            ""items"": [{ ""name"": ""c#"", ""count"": 200 }],
            ""has_more"": false
        }";

        using var db = GetDbContext();
        db.Tags.Add(new Tag { Name = "c#", Count = 100, LastUpdated = DateTime.UtcNow.AddDays(-1) });
        await db.SaveChangesAsync();

        var client = GetFakeHttpClient(json);
        var service = new StackOverflowService(db, client);

        // Act
        var fetched = await service.FetchTagsAsync(1);

        // Assert
        var updatedTag = db.Tags.First(t => t.Name == "c#");
        Assert.Equal(200, updatedTag.Count);
        Assert.True(updatedTag.LastUpdated > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task FetchTagsAsync_ShouldInsertNewTag_WhenNotExists()
    {
        // Arrange
        var json = @"{
            ""items"": [{ ""name"": ""python"", ""count"": 500 }],
            ""has_more"": false
        }";

        using var db = GetDbContext();
        var client = GetFakeHttpClient(json);

        // Act
        var service = new StackOverflowService(db, client);
        await service.FetchTagsAsync(1);

        // Assert
        var insertedTag = db.Tags.FirstOrDefault(t => t.Name == "python");
        Assert.NotNull(insertedTag);
        Assert.Equal(500, insertedTag.Count);
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        public FakeHttpMessageHandler(string response) => _response = response;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var msg = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_response, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(msg);
        }
    }
}
