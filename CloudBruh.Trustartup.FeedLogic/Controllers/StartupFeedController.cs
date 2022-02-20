using CloudBruh.Trustartup.FeedLogic.Models;
using CloudBruh.Trustartup.FeedLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudBruh.Trustartup.FeedLogic.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StartupFeedController : ControllerBase
{
    private readonly FeedContentService _feedContentService;
    private readonly UserService _userService;
    private readonly MediaService _mediaService;

    public StartupFeedController(FeedContentService feedContentService, UserService userService, MediaService mediaService)
    {
        _feedContentService = feedContentService;
        _userService = userService;
        _mediaService = mediaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StartupFeedItem>>> GetFeed(int count = 20, double? maxRating = null)
    {
        List<StartupRawDto> startups = (await _feedContentService.GetStartupsAsync(count, maxRating))?.ToList()
                                       ?? new List<StartupRawDto>();

        Dictionary<long, UserRawDto?> users = startups
            .Select(dto => dto.UserId)
            .Distinct()
            .Select(userId => (userId, _userService.GetUserAsync(userId).Result))
            .ToDictionary(tuple => tuple.userId, tuple => tuple.Result);

        return startups.Select(dto =>
        {
            users.TryGetValue(dto.UserId, out UserRawDto? user);

            MediaRelationshipRawDto? thumbnailRelation = _feedContentService
                .GetMediaRelationshipsAsync(MediableType.Startup, dto.Id).Result?.FirstOrDefault();
            MediaRawDto? thumbnail = thumbnailRelation != null
                ? _mediaService.GetMediumAsync(thumbnailRelation.MediaId).Result
                : null;
            
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
                ThumbnailLink = thumbnail?.Link
            };
        }).ToList();
    }
}