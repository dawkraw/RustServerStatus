using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using RustServerStatus.Models;
using RustServerStatus.Services;

namespace RustServerStatus.Controllers;

public class HomeController : Controller
{
    private readonly IAppCache _cache;
    private readonly IWebHostEnvironment _environment;
    private readonly IImageService _imageService;
    private readonly IServerQueryService _serverQueryService;

    public HomeController(IServerQueryService serverQueryService, IImageService imageService, IAppCache cache, IWebHostEnvironment environment)
    {
        _serverQueryService = serverQueryService;
        _imageService = imageService;
        _cache = cache;
        _environment = environment;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("ServerInfo")]
    public async Task<IActionResult> ServerInfo([FromQuery] [Required] string address)
    {
        if (_cache.TryGetValue(address.ToLowerInvariant(), out ServerInfo serverInfo)) 
            return View(serverInfo);
        
        serverInfo = await _serverQueryService.QueryServerAsync(address);
        
        if (serverInfo is null) 
            return RedirectToAction("Error");
        
        _cache.Add(address.ToLowerInvariant(), serverInfo, TimeSpan.FromMinutes(15));

        return View(serverInfo);
    }

    [Route("ServerImage/{address}")]
    [ResponseCache(Duration = 900, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> ServerImage([Required] string address)
    {
        if (_cache.TryGetValue(address.ToLowerInvariant(), out ServerInfo serverInfo))
            return File(_imageService.GenerateServerInfoImage(serverInfo), "image/png");
        
        serverInfo = await _serverQueryService.QueryServerAsync(address);
        
        if (serverInfo is null) 
            return PhysicalFile(_environment.WebRootFileProvider.GetFileInfo("img/NotAvailable.png")?.PhysicalPath, "image/png");
        
        _cache.Add(address.ToLowerInvariant(), serverInfo, TimeSpan.FromMinutes(15));
        
        
        return File(_imageService.GenerateServerInfoImage(serverInfo), "image/png");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}