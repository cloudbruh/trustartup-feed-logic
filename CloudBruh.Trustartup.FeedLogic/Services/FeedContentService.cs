using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using CloudBruh.Trustartup.FeedLogic.Models;

namespace CloudBruh.Trustartup.FeedLogic.Services;

public class FeedContentService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = {new JsonStringEnumConverter()}
    };
    
    private readonly ILogger<FeedContentService> _logger;
    private readonly HttpClient _httpClient;

    public FeedContentService(ILogger<FeedContentService> logger, HttpClient httpClient, IConfiguration config)
    {
        _logger = logger;
        _httpClient = httpClient;
        
        _httpClient.BaseAddress = new Uri(config.GetValue<string>("Settings:FeedContentSystemUrl"));
        _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue {NoCache = true};
    }

    public async Task<IEnumerable<StartupRawDto>?> GetStartupsAsync(int offset = 0, int count = 20, double? maxRating = null)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<StartupRawDto>>(maxRating == null
                ? $"api/Startup?offset={offset}&count={count}"
                : $"api/Startup?offset={offset}&count={count}&maxRating={maxRating}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve startups, {Exception}", e.Message);
            return null;
        }
    }

    public async Task<StartupRawDto?> GetStartupAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<StartupRawDto>($"api/Startup/{id}", SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve startup with id {Id}, {Exception}", id, e.Message);
            return null;
        }
    }
    
    public async Task<IEnumerable<MediaRelationshipRawDto>?> GetMediaRelationshipsAsync(MediableType? mediableType = null, long? mediableId = null)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<MediaRelationshipRawDto>>(mediableType == null
                ? "api/MediaRelationship"
                : $"api/MediaRelationship?mediableType={mediableType}&mediableId={mediableId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve media relationships, {Exception}", e.Message);
            return null;
        }
    }

    public async Task<MediaRelationshipRawDto?> GetMediaRelationshipAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MediaRelationshipRawDto>($"api/MediaRelationship/{id}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve media relationship with id {Id}, {Exception}", id, e.Message);
            return null;
        }
    }

    public async Task<bool> CheckStartupPublished(long id)
    {
        StartupRawDto? startupDto = await GetStartupAsync(id);

        return startupDto is not null && startupDto.Status == StartupStatus.Published;
    }
    
    public async Task<IEnumerable<PostRawDto>?> GetPostsAsync(long? startupId = null)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PostRawDto>>(startupId == null
                ? "api/Post"
                : $"api/Post?startupId={startupId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve posts, {Exception}", e.Message);
            return null;
        }
    }

    public async Task<PostRawDto?> GetPostAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PostRawDto>($"api/Post/{id}", SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve post with id {Id}, {Exception}", id, e.Message);
            return null;
        }
    }
    
    public async Task<IEnumerable<LikeRawDto>?> GetLikesAsync(LikeableType? likeableType = null, long? likeableId = null)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<LikeRawDto>>(likeableType == null
                    ? "api/Like"
                    : $"api/Like?likeableType={likeableType}&likeableId={likeableId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve likes, {Exception}", e.Message);
            return null;
        }
    }
    
    public async Task<long?> GetLikesCountAsync(LikeableType likeableType, long likeableId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<long>(
                $"api/Like/count?likeableType={likeableType}&likeableId={likeableId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve like count, {Exception}", e.Message);
            return null;
        }
    }
    
    public async Task<bool?> GetLikeCheckAsync(LikeableType likeableType, long likeableId, long userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<bool>(
                $"api/Like/check?likeableType={likeableType}&likeableId={likeableId}&userId={userId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not check like, {Exception}", e.Message);
            return null;
        }
    }

    public async Task<LikeRawDto?> PostLikeAsync(LikeRawDto likeRawDto)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Like", likeRawDto);
            return JsonSerializer.Deserialize<LikeRawDto>(await response.Content.ReadAsStreamAsync(), SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not post the like, {Exception}", e.Message);
            return null;
        }
    }
    
    public async Task<bool> DeleteLikeAsync(LikeableType likeableType, long likeableId, long userId)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Like?likeableType={likeableType}&likeableId={likeableId}&userId={userId}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not delete the like, {Exception}", e.Message);
            return false;
        }
    }
    
    public async Task<IEnumerable<FollowRawDto>?> GetFollowsAsync(long? startupId = null)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<FollowRawDto>>(startupId == null
                    ? "api/Follow"
                    : $"api/Follow?startupId={startupId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve follows, {Exception}", e.Message);
            return null;
        }
    }
    
    public async Task<long?> GetFollowsCountAsync(long startupId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<long>(
                $"api/Follow/count?startupId={startupId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve follow count, {Exception}", e.Message);
            return null;
        }
    }
    
    public async Task<bool?> GetFollowCheckAsync(long startupId, long userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<bool>(
                $"api/Follow/check?startupId={startupId}&userId={userId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not check follow, {Exception}", e.Message);
            return null;
        }
    }

    public async Task<FollowRawDto?> PostFollowAsync(FollowRawDto followRawDto)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Follow", followRawDto);
            return JsonSerializer.Deserialize<FollowRawDto>(await response.Content.ReadAsStreamAsync(), SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not post the follow, {Exception}", e.Message);
            return null;
        }
    }
    
    public async Task<bool> DeleteFollowAsync(long startupId, long userId)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Follow?startupId={startupId}&userId={userId}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not delete the follow, {Exception}", e.Message);
            return false;
        }
    }
    
    public async Task<IEnumerable<CommentRawDto>?> GetCommentsAsync(CommentableType? commentableType = null, long? commentableId = null)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CommentRawDto>>(commentableType == null
                    ? "api/Comment"
                    : $"api/Comment?commentableType={commentableType}&commentableId={commentableId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve comments, {Exception}", e.Message);
            return null;
        }
    }

    public async Task<CommentRawDto?> GetCommentAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CommentRawDto>($"api/Comment/{id}");
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve comment with id {Id}, {Exception}", id, e.Message);
            return null;
        }
    }

    public async Task<CommentRawDto?> PostCommentAsync(CommentRawDto commentRawDto)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Comment", commentRawDto);
            return JsonSerializer.Deserialize<CommentRawDto>(await response.Content.ReadAsStreamAsync(), SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not post the comment, {Exception}", e.Message);
            return null;
        }
    }
    
    public async Task<IEnumerable<RewardRawDto>?> GetRewardsAsync(long? startupId = null)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RewardRawDto>>(startupId == null
                    ? "api/Reward"
                    : $"api/Reward?startupId={startupId}",
                SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve rewards, {Exception}", e.Message);
            return null;
        }
    }

    public async Task<RewardRawDto?> GetRewardAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<RewardRawDto>($"api/Reward/{id}", SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve reward with id {Id}, {Exception}", id, e.Message);
            return null;
        }
    }
}