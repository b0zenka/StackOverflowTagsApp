using Domain.Entities;

namespace Application.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetTagsAsync(int page, int size, string sortBy, string order);
    Task<int> GetTotalTagsAsync();
}
