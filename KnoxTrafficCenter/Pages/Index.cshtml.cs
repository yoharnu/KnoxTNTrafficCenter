using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KnoxTrafficCenter.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, JsonFileCameraService cameraService, TDOTAPIService tdotApiService) : PageModel
    {
        public JsonFileCameraService CameraService = cameraService;
        public TDOTAPIService TDOTAPIService = tdotApiService;
        public IEnumerable<Camera> Cameras { get; private set; }
        public TDOTAPI TDOTAPI { get; private set; }

        public void OnGet()
        {
            Cameras = CameraService.GetCameras();

            if (TDOTAPIService.TDOTAPI is not null)
                TDOTAPI = TDOTAPIService.TDOTAPI;
        }
    }
}