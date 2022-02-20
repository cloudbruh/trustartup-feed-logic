using CloudBruh.Trustartup.FeedLogic.Models;
using CloudBruh.Trustartup.FeedLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudBruh.Trustartup.FeedLogic.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StartupFeedController : ControllerBase
{
    private readonly ILogger<StartupFeedController> _logger;
    private readonly FeedContentService _feedContentService;
    private readonly UserService _userService;

    public StartupFeedController(ILogger<StartupFeedController> logger, FeedContentService feedContentService, UserService userService)
    {
        _logger = logger;
        _feedContentService = feedContentService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StartupFeedItem>>> GetFeed(int count = 20, double? maxRating = null)
    {
        List<StartupRawDto> startups = (await _feedContentService.GetStartupsAsync(count, maxRating))?.ToList()
                                       ?? new List<StartupRawDto>();

        Dictionary<long, UserRawDto?> users = startups
            .Select(dto => dto.UserId)
            .Distinct()
            .Select(userId =>
            {
                try
                {
                    return (userId, _userService.GetUserAsync(userId).Result);
                }
                catch (Exception e)
                {
                    _logger.LogError("Could not retrieve user with id {UserId}, {Exception}", userId, e.Message);
                    return (userId, null);
                }
            })
            .ToDictionary(tuple => tuple.userId, tuple => tuple.Result);

        return startups.Select(dto =>
        {
            users.TryGetValue(dto.UserId, out UserRawDto? user);
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
        }).ToList();
    }
}