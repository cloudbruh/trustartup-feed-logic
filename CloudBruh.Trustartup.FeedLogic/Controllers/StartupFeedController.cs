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
    private readonly PaymentService _paymentService;

    public StartupFeedController(
        FeedContentService feedContentService,
        UserService userService,
        MediaService mediaService,
        PaymentService paymentService)
    {
        _feedContentService = feedContentService;
        _userService = userService;
        _mediaService = mediaService;
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StartupFeedItem>>> GetFeed(int offset = 0, int count = 20, double? maxRating = null)
    {
        List<StartupRawDto> startups = (await _feedContentService.GetStartupsAsync(offset, count, maxRating))?.ToList()
                                       ?? new List<StartupRawDto>();

        bool loggedIn = long.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "uid")?.Value, out long loggedUserId);

        Dictionary<long, UserRawDto?> users = startups
            .Where(dto => dto.Status == StartupStatus.Published)
            .Select(dto => dto.UserId)
            .Distinct()
            .Select(userId => (userId, _userService.GetUserAsync(userId).Result))
            .ToDictionary(tuple => tuple.userId, tuple => tuple.Result);

        return startups
            .Where(dto => dto.Status == StartupStatus.Published)
            .Select(dto =>
        {
            users.TryGetValue(dto.UserId, out UserRawDto? user);

            MediaRelationshipRawDto? thumbnailRelation = _feedContentService
                .GetMediaRelationshipsAsync(MediableType.Startup, dto.Id).Result?.FirstOrDefault();
            MediaRawDto? thumbnail = thumbnailRelation != null
                ? _mediaService.GetMediumAsync(thumbnailRelation.MediaId).Result
                : null;

            long likes = _feedContentService.GetLikesCountAsync(LikeableType.Startup, dto.Id).Result ?? 0;
            long follows = _feedContentService.GetFollowsCountAsync(dto.Id).Result ?? 0;

            var liked = false;
            var followed = false;
            if (loggedIn)
            {
                liked = _feedContentService.GetLikeCheckAsync(LikeableType.Startup, dto.Id, loggedUserId).Result ?? false;
                followed = _feedContentService.GetFollowCheckAsync(dto.Id, loggedUserId).Result ?? false;
            }

            decimal funded = _paymentService.GetPaymentCountAsync(dto.Id).Result ?? 0;
            
            return new StartupFeedItem
            {
                Id = dto.Id,
                Name = dto.Name,
                DescriptionShort = dto.Description,
                UserId = dto.UserId,
                UserName = user?.Name ?? dto.UserId.ToString(),
                UserSurname = user?.Surname ?? "",
                EndingAt = dto.EndingAt,
                FundsGoal = dto.FundsGoal,
                TotalFunded = funded,
                Rating = dto.Rating,
                Likes = likes,
                Follows = follows,
                Liked = liked,
                Followed = followed,
                ThumbnailLink = thumbnail?.Link
            };
        }).ToList();
    }
}