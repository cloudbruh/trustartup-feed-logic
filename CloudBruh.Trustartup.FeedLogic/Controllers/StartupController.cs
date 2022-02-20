using CloudBruh.Trustartup.FeedLogic.Models;
using CloudBruh.Trustartup.FeedLogic.Services;
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
            ImageLinks = images,
            UpdatedAt = dto.UpdatedAt,
            CreatedAt = dto.CreatedAt
        };
    }

    [HttpGet("{startupId:long}/posts")]
    public async Task<ActionResult<List<Post>>> GetStartupPosts(long startupId)
    {
        return (await _feedContentService.GetPostsAsync(startupId) ?? Array.Empty<PostRawDto>())
            .Select(dto => 
            {
                List<string> images = (_feedContentService.GetMediaRelationshipsAsync(MediableType.Post, dto.Id).Result
                                       ?? Array.Empty<MediaRelationshipRawDto>())
                    .Select(relation => _mediaService.GetMediumAsync(relation.MediaId).Result?.Link)
                    .OfType<string>()
                    .ToList();

                return new Post
                {
                    Id = dto.Id,
                    StartupId = dto.StartupId,
                    Header = dto.Header,
                    Text = dto.Text,
                    ImageLinks = images,
                    UpdatedAt = dto.UpdatedAt,
                    CreatedAt = dto.CreatedAt
                };
            }).ToList();
    }
}