using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CloudBruh.Trustartup.FeedLogic.Models;
using CloudBruh.Trustartup.FeedLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudBruh.Trustartup.FeedLogic.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StartupFeedController : ControllerBase
{
    private readonly FeedContentService _feedContentService;

    public StartupFeedController(FeedContentService feedContentService)
    {
        _feedContentService = feedContentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StartupFeedItem>>> GetFeed(int count = 20, double? maxRating = null)
    {
        IEnumerable<StartupRawDto>? startups = await _feedContentService.GetStartupsAsync(count, maxRating);

        return startups?.Select(dto => new StartupFeedItem
        {
            Id = dto.Id,
            Name = dto.Name,
            DescriptionShort = dto.Description,
            UserId = dto.UserId,
            EndingAt = dto.EndingAt,
            FundsGoal = dto.FundsGoal,
            Rating = dto.Rating,
            ThumbnailLink = ""
        }).ToList() ?? new List<StartupFeedItem>();
    }
}