using CloudBruh.Trustartup.FeedLogic.Models;

namespace CloudBruh.Trustartup.FeedLogic.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        
        _httpClient.BaseAddress = new Uri(config.GetValue<string>("Settings:UserSystemUrl"));
    }
    
    public async Task<IEnumerable<UserRawDto>?> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<UserRawDto>>("users");
    }

    public async Task<UserRawDto?> GetUserAsync(long id)
    {
        return await _httpClient.GetFromJsonAsync<UserRawDto>($"users/{id}");
    }
}