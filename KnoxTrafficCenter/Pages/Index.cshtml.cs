using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Models.TDOT;
using KnoxTrafficCenter.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KnoxTrafficCenter.Pages;

public class IndexModel(ILogger<IndexModel> logger, TDOTAPIService tdotApiService) : PageModel
{
    public List<CameraGroup> CameraGroups { get; private set; } = [];
    public List<Event> Incidents { get; private set; } = [];
    public List<Event> ConstructionEvents { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var api = await tdotApiService.GetTDOTAPIAsync();
        CameraGroups = await tdotApiService.GetCamerasAsync();
        Incidents = await tdotApiService.GetIncidentsAsync();
        ConstructionEvents = await tdotApiService.GetConstructionAsync();
    }
}