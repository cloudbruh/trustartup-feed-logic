using CloudBruh.Trustartup.FeedLogic.Models;

namespace CloudBruh.Trustartup.FeedLogic.Services;

public class FeedContentService
{
    private readonly HttpClient _httpClient;

    public FeedContentService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        
        _httpClient.BaseAddress = new Uri(config.GetValue<string>("Settings:FeedContentSystemUrl"));
    }

    public async Task<IEnumerable<StartupRawDto>?> GetStartupsAsync(int count = 20, double? maxRating = null)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<StartupRawDto>>(maxRating == null
            ? $"api/Startup?count={count}"
            : $"api/Startup?count={count}&maxRating={maxRating}");
    }
}