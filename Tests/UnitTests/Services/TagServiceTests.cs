using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;

namespace UnitTests.Services;

public class TagServiceTests
{
    [Fact]
    public async Task GetTagsAsync_ShouldFetchFromSO_WhenNoTagsInDb()
    {
        // Arrange
        var repoMock = new Mock<ITagRepository>();
        repoMock.Setup(r => r.GetTotalTagsAsync()).ReturnsAsync(0);

        var soMock = new Mock<IStackOverflowService>();
        soMock.Setup(s => s.FetchTagsAsync(It.IsAny<int>(), default)).ReturnsAsync(10);

        var service = new TagService(repoMock.Object, soMock.Object);

        // Act
        var result = await service.GetTagsAsync(1, 10, "name", "asc");

        // Assert
        soMock.Verify(s => s.FetchTagsAsync(10, default), Times.Once);
    }

    [Fact]
    public async Task RefreshTagsAsync_ShouldCallSOService()
    {
        var repoMock = new Mock<ITagRepository>();
        var soMock = new Mock<IStackOverflowService>();

        var service = new TagService(repoMock.Object, soMock.Object);

        await service.RefreshTagsAsync(1000);

        soMock.Verify(s => s.FetchTagsAsync(1000, default), Times.Once);
    }

    [Fact]
    public async Task GetTagsAsync_ShouldNotCallSOService_WhenDbHasTags()
    {
        // Arrange
        var repoMock = new Mock<ITagRepository>();
        repoMock.Setup(r => r.GetTotalTagsAsync()).ReturnsAsync(5);
        repoMock.Setup(r => r.GetTagsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Tag> { new Tag { Name = "c#", Count = 10 } });

        var soMock = new Mock<IStackOverflowService>();

        var service = new TagService(repoMock.Object, soMock.Object);

        // Act
        var tags = await service.GetTagsAsync(1, 10, "name", "asc");

        // Assert
        Assert.Single(tags);
        soMock.Verify(s => s.FetchTagsAsync(It.IsAny<int>(), default), Times.Never);
    }
}
