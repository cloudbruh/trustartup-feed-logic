using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using CloudBruh.Trustartup.FeedLogic.Models;

namespace CloudBruh.Trustartup.FeedLogic.Services;

public class MediaService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = {new JsonStringEnumConverter()}
    };
    
    private readonly ILogger<MediaService> _logger;
    private readonly HttpClient _httpClient;

    public MediaService(ILogger<MediaService> logger, HttpClient httpClient, IConfiguration config)
    {
        _logger = logger;
        _httpClient = httpClient;
        
        _httpClient.BaseAddress = new Uri(config.GetValue<string>("Settings:MediaSystemUrl"));
        _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue {NoCache = true};
    }
    
    public async Task<MediaRawDto?> GetMediaAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MediaRawDto>($"api/Media", SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve media, {Exception}", e.Message);
            return null;
        }
    }
    
    public async Task<MediaRawDto?> GetMediumAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MediaRawDto>($"api/Media/{id}", SerializerOptions);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve medium with id {Id}, {Exception}", id, e.Message);
            return null;
        }
    }
}