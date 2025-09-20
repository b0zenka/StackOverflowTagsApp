using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

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

            var names = response.Items.Select(x => x.Name).ToList();
            var existingTags = await _db.Tags
                .Where(t => names.Contains(t.Name))
                .ToDictionaryAsync(t => t.Name, cancellationToken);

            var now = DateTime.UtcNow;

            foreach (var it in response.Items)
            {
                if (existingTags.TryGetValue(it.Name, out var existing))
                {
                    if (existing.Count != it.Count)
                    {
                        existing.Count = it.Count;
                        existing.LastUpdated = now;
                    }
                }
                else
                    _db.Tags.Add(new Tag
                    {
                        Name = it.Name,
                        Count = it.Count,
                        LastUpdated = now
                    });
            }

            await _db.SaveChangesAsync(cancellationToken);

            totalFetched += response.Items.Count;
            page++;

            if (!response.HasMore)
                break;

            if (response.Backoff is int backoff)
                await Task.Delay(TimeSpan.FromSeconds(backoff), cancellationToken);
            else
                await Task.Delay(300, cancellationToken);
        }

        return totalFetched;
    }


    private class TagApiResponse
    {
        [JsonPropertyName("items")]
        public List<TagDto> Items { get; set; } = new();

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("backoff")]
        public int? Backoff { get; set; }

        [JsonPropertyName("quota_remaining")]
        public int QuotaRemaining { get; set; }
    }

    private class TagDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}