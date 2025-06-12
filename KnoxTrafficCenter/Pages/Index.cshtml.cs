using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Models.TDOT;
using KnoxTrafficCenter.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KnoxTrafficCenter.Pages;

public class IndexModel(TDOTAPIService tdotApiService) : PageModel
{
    public List<CameraGroup> CameraGroups { get; set; } = [];

    public async Task OnGetAsync()
    {
        CameraGroups = await tdotApiService.GetCamerasAsync();
    }
}