using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Repository;

public class TagRepositoryTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetTagsAsync_ShouldReturnTagsWithShares()
    {
        // Arrange
        using var db = GetDbContext();
        db.Tags.AddRange(
            new Tag { Name = "tag_test1", Count = 100 },
            new Tag { Name = "tag_test2", Count = 50 }
        );
        await db.SaveChangesAsync();

        var repo = new TagRepository(db);

        // Act
        var result = await repo.GetTagsAsync(1, 10, "name", "asc");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.True(result.All(t => t.Share > 0));
    }

    [Fact]
    public async Task GetTagsAsync_ShouldSortByNameDescending()
    {
        // Arrange
        using var db = GetDbContext();
        db.Tags.AddRange(
            new Tag { Name = "csharp", Count = 10 },
            new Tag { Name = "java", Count = 20 }
        );
        await db.SaveChangesAsync();

        var repo = new TagRepository(db);

        // Act
        var result = await repo.GetTagsAsync(1, 10, "name", "desc");

        // Assert
        var ordered = result.ToList();
        Assert.Equal("java", ordered.First().Name);
    }

    [Fact]
    public async Task GetTagsAsync_ShouldReturnSecondPage()
    {
        // Arrange
        using var db = GetDbContext();
        db.Tags.AddRange(
            new Tag { Name = "tag_test1", Count = 10 },
            new Tag { Name = "tag_test2", Count = 20 },
            new Tag { Name = "tag_test3", Count = 30 }
        );
        await db.SaveChangesAsync();

        var repo = new TagRepository(db);

        // Act
        var result = await repo.GetTagsAsync(2, 1, "name", "asc");

        // Assert
        Assert.Single(result);
        Assert.Equal("tag_test2", result.First().Name);
    }

}
