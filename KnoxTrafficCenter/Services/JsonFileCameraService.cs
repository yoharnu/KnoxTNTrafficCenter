using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using KnoxTrafficCenter.Models;
using Microsoft.AspNetCore.Hosting;

namespace KnoxTrafficCenter.Services
{
    public class JsonFileCameraService
    {
        public JsonFileCameraService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        private string JsonFileName
        {
            get { return Path.Combine(WebHostEnvironment.ContentRootPath, "data", "cameras.json"); }
        }

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
