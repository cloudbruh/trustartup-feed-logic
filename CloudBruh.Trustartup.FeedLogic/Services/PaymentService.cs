namespace CloudBruh.Trustartup.FeedLogic.Services;

public class PaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly HttpClient _httpClient;

    public PaymentService(ILogger<PaymentService> logger, HttpClient httpClient, IConfiguration config)
    {
        _logger = logger;
        _httpClient = httpClient;
        
        _httpClient.BaseAddress = new Uri(config.GetValue<string>("Settings:PaymentSystemUrl"));
    }
    
    public async Task<decimal?> GetPaymentCountAsync(long startupId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<decimal>($"payment/sum?startup_id={startupId}");
        }
        catch (HttpRequestException e)
        {
            _logger.LogError("Could not retrieve payment count, {Exception}", e.Message);
            return null;
        }
    }

}