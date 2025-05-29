using System.Text.Json.Serialization;

namespace KnoxTrafficCenter.Models
{
    public class Camera
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Road { get; set; }
        public string? Location { get; set; }
        public float? MM { get; set; }
        public Media Video { get; set; }
        public Media Image { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize<Camera>(this);
        }

        [JsonConstructor]
        public Camera() { }

        public Camera(TDOT.Camera camera)
        {
            Id = camera.Id;
            Title = camera.Title ?? camera.Route + " " + camera.Jurisdiction;
            MM = string.IsNullOrEmpty(camera.MileMarker) ? null : float.Parse(camera.MileMarker);
            Video = new Media
            {
                URL = camera.HttpsVideoUrl,
                Width = 360, // Default width, can be adjusted
                Height = 240 // Default height, can be adjusted
            };
            Image = new Media
            {
                URL = camera.ThumbnailUrl,
                Width = 320, // Default width, can be adjusted
                Height = 240 // Default height, can be adjusted
            };
        }
    }

    public class Media
    {
        public string URL { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
