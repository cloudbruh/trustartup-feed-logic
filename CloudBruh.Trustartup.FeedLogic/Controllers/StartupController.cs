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
    private readonly PaymentService _paymentService;

    public StartupController(
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

        decimal funded = await _paymentService.GetPaymentCountAsync(dto.Id) ?? 0;
        
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
            TotalFunded = funded,
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
    public async Task<ActionResult<LikesInfo>> PostLike(long id)
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
        if (result == null)
        {
            return BadRequest("Failed to like");
        }

        long likes = await _feedContentService.GetLikesCountAsync(LikeableType.Startup, id) ?? 0;
        bool liked = await _feedContentService.GetLikeCheckAsync(LikeableType.Startup, id, userId) ?? false;

        return new LikesInfo()
        {
            LikeableType = LikeableType.Startup,
            LikeableId = id,
            Likes = likes,
            Liked = liked
        };
    }
    
    [Authorize]
    [HttpDelete("{id:long}/like")]
    public async Task<ActionResult<LikesInfo>> DeleteLike(long id)
    {
        if (!long.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "uid")?.Value, out long userId))
        {
            return BadRequest("Invalid uid in jwt token.");
        }

        if (!await _feedContentService.DeleteLikeAsync(LikeableType.Startup, id, userId))
        {
            return BadRequest("Failed to remove like");
        }
        
        long likes = await _feedContentService.GetLikesCountAsync(LikeableType.Startup, id) ?? 0;
        bool liked = await _feedContentService.GetLikeCheckAsync(LikeableType.Startup, id, userId) ?? false;

        return new LikesInfo()
        {
            LikeableType = LikeableType.Startup,
            LikeableId = id,
            Likes = likes,
            Liked = liked
        };
    }
    
    [Authorize]
    [HttpPost("{id:long}/follow")]
    public async Task<ActionResult<FollowsInfo>> PostFollow(long id)
    {
        if (!long.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "uid")?.Value, out long userId))
        {
            return BadRequest("Invalid uid in jwt token.");
        }

        var dto = new FollowRawDto()
        {
            UserId = userId,
            StartupId = id,
        };

        FollowRawDto? result = await _feedContentService.PostFollowAsync(dto);
        if (result == null)
        {
            return BadRequest("Failed to follow");
        }
        
        long follows = await _feedContentService.GetFollowsCountAsync(id) ?? 0;
        bool followed = await _feedContentService.GetFollowCheckAsync(id, userId) ?? false;

        return new FollowsInfo()
        {
            StartupId = id,
            Follows = follows,
            Followed = followed
        };
    }
    
    [Authorize]
    [HttpDelete("{id:long}/follow")]
    public async Task<ActionResult<FollowsInfo>> DeleteFollow(long id)
    {
        if (!long.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "uid")?.Value, out long userId))
        {
            return BadRequest("Invalid uid in jwt token.");
        }

        if (!await _feedContentService.DeleteFollowAsync(id, userId))
        {
            return BadRequest("Failed to remove follow");
        }

        long follows = await _feedContentService.GetFollowsCountAsync(id) ?? 0;
        bool followed = await _feedContentService.GetFollowCheckAsync(id, userId) ?? false;

        return new FollowsInfo()
        {
            StartupId = id,
            Follows = follows,
            Followed = followed
        };
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

    [Authorize]
    [HttpPost("{startupId:long}/comment")]
    public async Task<ActionResult<Comment>> PostComment(long startupId, CommentCreation creation)
    {
        if (!long.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "uid")?.Value, out long userId))
        {
            return BadRequest("Invalid uid in jwt token.");
        }

        var dto = new CommentRawDto()
        {
            UserId = userId,
            CommentableId = startupId,
            CommentableType = CommentableType.Startup,
            RepliedId = creation.RepliedId,
            Text = creation.Text
        };

        dto = await _feedContentService.PostCommentAsync(dto);
        if (dto == null)
        {
            return BadRequest("Failed to create comment");
        }
        
        UserRawDto? user = await _userService.GetUserAsync(userId);

        return new Comment()
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
    }
    
    [HttpGet("{startupId:long}/comments")]
    public async Task<ActionResult<List<Reward>>> GetStartupRewards(long startupId)
    {
        List<RewardRawDto> rewards = (await _feedContentService.GetRewardsAsync(startupId))?.ToList()
                                      ?? new List<RewardRawDto>();

        return rewards.Select(dto =>
        {
            MediaRawDto? media = _mediaService.GetMediumAsync(dto.MediaId).Result;
            
            return new Reward
            {
                Id = dto.Id,
                StartupId = dto.StartupId,
                Name = dto.Name,
                DonationMinimum = dto.DonationMinimum,
                MediaLink = media?.Link,
                Description = dto.Description,
                UpdatedAt = dto.UpdatedAt,
                CreatedAt = dto.CreatedAt
            };
        }).ToList();
    }
}