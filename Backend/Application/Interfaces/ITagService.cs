using Domain.Entities;

namespace Application.Interfaces;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetTagsAsync(int page, int size, string sortBy, string order, CancellationToken cancellationToken = default);
    Task<int> GetTotalTagsAsync();
    Task RefreshTagsAsync(int minTags = 1000, CancellationToken cancellationToken = default);
}