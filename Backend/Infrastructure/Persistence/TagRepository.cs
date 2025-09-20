using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _dbContext;

    public TagRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Tag>> GetTagsAsync(int page, int size, string sortBy, string order)
    {
        var total = await _dbContext.Tags.SumAsync(t => t.Count);

        var query = _dbContext.Tags.AsQueryable();

        query = sortBy switch
        {
            "name" => order == "asc" ? query.OrderBy(t => t.Name) : query.OrderByDescending(t => t.Name),
            "share" => order == "asc"
                ? query.OrderBy(t => (double)t.Count / total)
                : query.OrderByDescending(t => (double)t.Count / total),
            _ => order == "asc" ? query.OrderBy(t => t.Count) : query.OrderByDescending(t => t.Count)
        };

        var tags = await query
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();

        foreach (var tag in tags)
        {
            tag.Share = total > 0
                ? (double)tag.Count / total * 100.0
                : 0;
        }

        return tags;
    }

    public Task<int> GetTotalTagsAsync() => _dbContext.Tags.CountAsync();
}
