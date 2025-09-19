using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTags(int page = 1, int size = 20, string sortBy = "name", string order = "asc", CancellationToken cancellationToken = default)
    {
        var tags = await _tagService.GetTagsAsync(page, size, sortBy, order, cancellationToken);
        var totalTags = await _tagService.GetTotalTagsAsync();
        var response = new
        {
            Data = tags,
            Page = page,
            Size = size,
            Total = totalTags
        };

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTags(int minTags = 1000, CancellationToken cancellationToken = default)
    {
        await _tagService.RefreshTagsAsync(minTags);
        return Ok(new { status = "refresh started" });
    }
}