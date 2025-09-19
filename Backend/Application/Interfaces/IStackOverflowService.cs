using System.Threading;

namespace Application.Interfaces;

public interface IStackOverflowService
{
    Task<int> FetchTagsAsync(int minTags = 1000, CancellationToken cancellationToken = default);
}
