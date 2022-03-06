using CloudBruh.Trustartup.FeedLogic.Models;
using CloudBruh.Trustartup.FeedLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudBruh.Trustartup.FeedLogic.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StartupController : ControllerBase
{
    private readonly FeedContentService _feedContentService;
    private readonly UserService _userService;
    private readonly MediaService _mediaService;

    public StartupController(FeedContentService feedContentService, UserService userService, MediaService mediaService)
    {
        _feedContentService = feedContentService;
        _userService = userService;
        _mediaService = mediaService;
    }

    // GET: api/Startup/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<StartupDetail>> GetStartup(long id)
    {
        StartupRawDto? dto = await _feedContentService.GetStartupAsync(id);

        if (dto is null)
        {
            return NotFound();
        }

        UserRawDto? user = await _userService.GetUserAsync(dto.UserId);

        List<string> images = (await _feedContentService.GetMediaRelationshipsAsync(MediableType.Startup, dto.Id)
                               ?? Array.Empty<MediaRelationshipRawDto>())
            .Select(relation => _mediaService.GetMediumAsync(relation.MediaId).Result?.Link)
            .OfType<string>()
            .ToList();
        
        long likes = _feedContentService.GetLikesCountAsync(LikeableType.Startup, dto.Id).Result ?? 0;
        long follows = _feedContentService.GetFollowsCountAsync(dto.Id).Result ?? 0;

        var liked = false;
        var followed = false;
        if (long.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "uid")?.Value, out long loggedUserId))
        {
            liked = await _feedContentService.GetLikeCheckAsync(LikeableType.Startup, dto.Id, loggedUserId) ?? false;
            followed = await _feedContentService.GetFollowCheckAsync(dto.Id, loggedUserId) ?? false;
        }
        
        return new StartupDetail
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            UserId = dto.UserId,
            UserName = user?.Name ?? "",
            UserSurname = user?.Surname ?? "",
            EndingAt = dto.EndingAt,
            FundsGoal = dto.FundsGoal,
            Rating = dto.Rating,
            Likes = likes,
            Follows = follows,
            Liked = liked,
            Followed = followed,
            ImageLinks = images,
            UpdatedAt = dto.UpdatedAt,
            CreatedAt = dto.CreatedAt
        };
    }

    [HttpGet("{startupId:long}/posts")]
    public async Task<ActionResult<List<Post>>> GetStartupPosts(long startupId)
    {
        bool loggedIn = long.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "uid")?.Value, out long loggedUserId);
        
        return (await _feedContentService.GetPostsAsync(startupId) ?? Array.Empty<PostRawDto>())
            .Select(dto => 
            {
                List<string> images = (_feedContentService.GetMediaRelationshipsAsync(MediableType.Post, dto.Id).Result
                                       ?? Array.Empty<MediaRelationshipRawDto>())
                    .Select(relation => _mediaService.GetMediumAsync(relation.MediaId).Result?.Link)
                    .OfType<string>()
                    .ToList();

                long likes = _feedContentService.GetLikesCountAsync(LikeableType.Post, dto.Id).Result ?? 0;
                
                var liked = false;
                if (loggedIn)
                {
                    liked = _feedContentService.GetLikeCheckAsync(LikeableType.Post, dto.Id, loggedUserId).Result ?? false;
                }
                
                return new Post
                {
                    Id = dto.Id,
                    StartupId = dto.StartupId,
                    Header = dto.Header,
                    Text = dto.Text,
                    Likes = likes,
                    Liked = liked,
                    ImageLinks = images,
                    UpdatedAt = dto.UpdatedAt,
                    CreatedAt = dto.CreatedAt
                };
            }).ToList();
    }
    
    [Authorize]
    [HttpPost("{id:long}/like")]
    public async Task<ActionResult<LikeRawDto>> PostLike(long id)
    {
        if (!long.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "uid")?.Value, out long userId))
        {
            return BadRequest("Invalid uid in jwt token.");
        }

        var dto = new LikeRawDto
        {
            UserId = userId,
            LikeableId = id,
            LikeableType = LikeableType.Startup
        };

        LikeRawDto? result = await _feedContentService.PostLikeAsync(dto);

        return result == null ? BadRequest("Failed to like") : result;
    }
    
    [HttpGet("{startupId:long}/comments")]
    public async Task<ActionResult<List<Comment>>> GetStartupComments(long startupId)
    {
        List<CommentRawDto> comments = (await _feedContentService.GetCommentsAsync(CommentableType.Startup, startupId))?.ToList()
                                       ?? new List<CommentRawDto>();
        
        Dictionary<long, UserRawDto?> users = comments
            .Select(dto => dto.UserId)
            .Distinct()
            .Select(userId => (userId, _userService.GetUserAsync(userId).Result))
            .ToDictionary(tuple => tuple.userId, tuple => tuple.Result);

        return comments.Select(dto =>
        {
            users.TryGetValue(dto.UserId, out UserRawDto? user);
            
            return new Comment
            {
                Id = dto.Id,
                UserId = dto.UserId,
                UserName = user?.Name ?? dto.UserId.ToString(),
                UserSurname = user?.Surname ?? "",
                CommentableId = dto.CommentableId,
                CommentableType = dto.CommentableType,
                RepliedId = dto.RepliedId,
                Text = dto.Text,
                UpdatedAt = dto.UpdatedAt,
                CreatedAt = dto.CreatedAt
            };
        }).ToList();
    }
}