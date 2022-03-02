using CloudBruh.Trustartup.FeedLogic.Models;

namespace CloudBruh.Trustartup.FeedLogic.Services;

public class UserService
{
    private readonly ILogger<UserService> _logger;
    private readonly HttpClient _httpClient;

    public UserService(ILogger<UserService> logger, HttpClient httpClient, IConfiguration config)
    {
        _logger = logger;
        _httpClient = httpClient;
        
        _httpClient.BaseAddress = new Uri(config.GetValue<string>("Settings:UserSystemUrl"));
    }
    
    public async Task<IEnumerable<UserRawDto>?> GetUsersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UserRawDto>>("user");
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve users, {Exception}", e.Message);
            return null;
        }
    }

    public async Task<UserRawDto?> GetUserAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserRawDto>($"user/{id}");
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve user with id {Id}, {Exception}", id, e.Message);
            return null;
        }
    }
}