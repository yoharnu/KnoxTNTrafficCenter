using KnoxTrafficCenter.Models.TDOT;
using KnoxTrafficCenter.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Camera = KnoxTrafficCenter.Models.Camera;

namespace KnoxTrafficCenter.Pages;

public class IndexModel(ILogger<IndexModel> logger, TDOTAPIService tdotApiService) : PageModel
{
    public List<Camera> Cameras { get; private set; } = [];
    public List<Event> Incidents { get; private set; } = [];
    public List<Event> ConstructionEvents { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var api = await tdotApiService.GetTDOTAPIAsync();
        Cameras = await tdotApiService.GetCamerasAsync();
        logger.LogDebug("Routes: " + string.Join(", ", Cameras.Select(x => x.Road).Distinct()));
        Incidents = await tdotApiService.GetIncidentsAsync();
        ConstructionEvents = await tdotApiService.GetConstructionAsync();
    }
}