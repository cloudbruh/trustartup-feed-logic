using CloudBruh.Trustartup.FeedLogic.Models;
using CloudBruh.Trustartup.FeedLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudBruh.Trustartup.FeedLogic.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly FeedContentService _feedContentService;
    private readonly UserService _userService;
    private readonly MediaService _mediaService;

    public PostController(FeedContentService feedContentService, UserService userService, MediaService mediaService)
    {
        _feedContentService = feedContentService;
        _userService = userService;
        _mediaService = mediaService;
    }
    
    // GET: api/Post/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<Post>> GetStartup(long id)
    {
        PostRawDto? dto = await _feedContentService.GetPostAsync(id);

        if (dto is null)
        {
            return NotFound();
        }

        List<string> images = (_feedContentService.GetMediaRelationshipsAsync(MediableType.Post, dto.Id).Result
                               ?? Array.Empty<MediaRelationshipRawDto>())
            .Select(relation => _mediaService.GetMediumAsync(relation.MediaId).Result?.Link)
            .OfType<string>()
            .ToList();

        long likes = _feedContentService.GetLikesCountAsync(LikeableType.Startup, dto.Id).Result ?? 0;
        
        return new Post
        {
            Id = dto.Id,
            StartupId = dto.StartupId,
            Header = dto.Header,
            Text = dto.Text,
            Likes = likes,
            ImageLinks = images,
            UpdatedAt = dto.UpdatedAt,
            CreatedAt = dto.CreatedAt
        };
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
            LikeableType = LikeableType.Post
        };

        LikeRawDto? result = await _feedContentService.PostLikeAsync(dto);

        return result == null ? BadRequest("Failed to like") : result;
    }
    
    [HttpGet("{postId:long}/comments")]
    public async Task<ActionResult<List<Comment>>> GetStartupComments(long postId)
    {
        List<CommentRawDto> comments = (await _feedContentService.GetCommentsAsync(CommentableType.Post, postId))?.ToList()
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