using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Infrastructure.Services;

public class StackOverflowService : IStackOverflowService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _http;

    public StackOverflowService(AppDbContext db, HttpClient http)
    {
        _db = db;
        _http = http;
    }

    public async Task<int> FetchTagsAsync(int minTags = 1000, CancellationToken cancellationToken = default)
    {
        int page = 1, totalFetched = 0;

        while (totalFetched < minTags && !cancellationToken.IsCancellationRequested)
        {
            var url =
                $"https://api.stackexchange.com/2.3/tags?page={page}&pagesize=100&order=desc&sort=popular&site=stackoverflow";

            var response = await _http.GetFromJsonAsync<TagApiResponse>(url, cancellationToken);

            if (response?.Items is null || response.Items.Count == 0)
                break;

            var existingTags = await _db.Tags
                .Where(t => response.Items.Select(x => x.Name).Contains(t.Name))
                .ToDictionaryAsync(t => t.Name, cancellationToken);

            foreach (var it in response.Items)
            {
                if (existingTags.TryGetValue(it.Name, out var existing))
                {
                    existing.Count = it.Count;
                    existing.LastUpdated = DateTime.UtcNow;
                }
                else
                {
                    _db.Tags.Add(new Tag
                    {
                        Name = it.Name,
                        Count = it.Count,
                        LastUpdated = DateTime.UtcNow
                    });
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            totalFetched = await _db.Tags.CountAsync(cancellationToken);
            page++;

            await Task.Delay(200, cancellationToken);
        }

        return totalFetched;
    }

    private class TagApiResponse
    {
        public List<TagDto> Items { get; set; } = new();
    }

    private class TagDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}