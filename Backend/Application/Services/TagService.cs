using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IStackOverflowService _stackOverflowService;

    public TagService(ITagRepository tagRepository, IStackOverflowService stackOverflowService)
    {
        _tagRepository = tagRepository;
        _stackOverflowService = stackOverflowService;
    }

    public async Task<IEnumerable<Tag>> GetTagsAsync(int page, int size, string sortBy, string order, CancellationToken cancellationToken = default)
    {
        var totalTagsCount = await _tagRepository.GetTotalTagsAsync();
        if (totalTagsCount == 0)
            await _stackOverflowService.FetchTagsAsync(size);
        
        return await _tagRepository.GetTagsAsync(page, size, sortBy, order);
    }

    public Task<int> GetTotalTagsAsync() => _tagRepository.GetTotalTagsAsync();

    public Task RefreshTagsAsync(int minTags = 1000, CancellationToken cancellationToken = default) => _stackOverflowService.FetchTagsAsync(minTags, cancellationToken);
}