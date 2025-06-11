using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Models.TDOT;
using KnoxTrafficCenter.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KnoxTrafficCenter.Pages;

public class IndexModel(ILogger<IndexModel> logger, TDOTAPIService tdotApiService) : PageModel
{
    public List<CameraGroup> CameraGroups { get; set; } = [];
    public List<Event> Incidents { get; set; } = [];
    public List<Event> ConstructionEvents { get; set; } = [];
    public List<Event> WeatherEvents { get; set; } = [];
    public List<Event> CountyWideWeatherEvents { get; set; } = [];

    public async Task OnGetAsync()
    {
        var api = await tdotApiService.GetTDOTAPIAsync();
        CameraGroups = await tdotApiService.GetCamerasAsync();
        Incidents = await tdotApiService.GetIncidentsAsync();
        ConstructionEvents = await tdotApiService.GetConstructionAsync();
        WeatherEvents = await tdotApiService.GetWeatherAsync();
        CountyWideWeatherEvents = await tdotApiService.GetCountyWideWeatherAsync();
    }
}