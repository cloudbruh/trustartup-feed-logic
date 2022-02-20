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
    }

    public async Task<IEnumerable<StartupRawDto>?> GetStartupsAsync(int count = 20, double? maxRating = null)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<StartupRawDto>>(maxRating == null
                ? $"api/Startup?count={count}"
                : $"api/Startup?count={count}&maxRating={maxRating}");
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
            return await _httpClient.GetFromJsonAsync<StartupRawDto>($"api/Startup/{id}");
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
}