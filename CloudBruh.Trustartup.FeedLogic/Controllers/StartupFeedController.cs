using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudBruh.Trustartup.FeedLogic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudBruh.Trustartup.FeedLogic.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StartupFeedController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public StartupFeedController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<StartupFeedItem>>> GetFeed(int count = 20, double? maxRating = null)
    {
        using HttpClient? httpClient = _httpClientFactory.CreateClient();
        
        httpClient.GetAsync()
    }
}