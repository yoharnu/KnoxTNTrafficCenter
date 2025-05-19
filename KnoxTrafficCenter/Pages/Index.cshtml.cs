using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KnoxTrafficCenter.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public JsonFileCameraService CameraService;
        public TDOTAPIService TDOTAPIService;
        public IEnumerable<Camera> Cameras { get; private set; }
        public TDOTAPI TDOTAPI { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, JsonFileCameraService cameraService, TDOTAPIService tdotApiService)
        {
            CameraService = cameraService;
            TDOTAPIService = tdotApiService;
            _logger = logger;
        }

        public void OnGet()
        {
            Cameras = CameraService.GetCameras();

            if (TDOTAPIService.TDOTAPI is not null)
                TDOTAPI = TDOTAPIService.TDOTAPI;
        }
    }
}