using KnoxTrafficCenter.Models;
using System.Text.Json;

namespace KnoxTrafficCenter.Services
{
    public class JsonFileCameraService(IWebHostEnvironment webHostEnvironment)
    {
        private IWebHostEnvironment WebHostEnvironment { get; } = webHostEnvironment;

        private string JsonFileName => Path.Combine(WebHostEnvironment.ContentRootPath, "data", "cameras.json");

        public IEnumerable<Camera> GetCameras()
        {
            using var jsonFileReader = File.OpenText(JsonFileName);
            return JsonSerializer.Deserialize<Camera[]>(jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
