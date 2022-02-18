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
    private readonly UserService _userService;

    public StartupFeedController(FeedContentService feedContentService, UserService userService)
    {
        _feedContentService = feedContentService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StartupFeedItem>>> GetFeed(int count = 20, double? maxRating = null)
    {
        List<StartupRawDto>? startups = (await _feedContentService.GetStartupsAsync(count, maxRating))?.ToList();

        Dictionary<long, UserRawDto?>? users = startups?.Select(dto => dto.UserId).Distinct()
            .Select(async userId => await _userService.GetUserAsync(userId)).Select(task => task.Result)
            .ToDictionary(dto => dto.Id);

        return startups?.Select(async dto =>
        {
            UserRawDto? user = await _userService.GetUserAsync(dto.UserId);
            return new StartupFeedItem
            {
                Id = dto.Id,
                Name = dto.Name,
                DescriptionShort = dto.Description,
                UserId = dto.UserId,
                UserName = user?.Name ?? "",
                UserSurname = user?.Surname ?? "",
                EndingAt = dto.EndingAt,
                FundsGoal = dto.FundsGoal,
                Rating = dto.Rating,
                ThumbnailLink = ""
            };
        }).Select(task => task.Result).ToList() ?? new List<StartupFeedItem>();
    }
}