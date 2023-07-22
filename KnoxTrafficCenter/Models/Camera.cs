using System.Text.Json;
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
    }

    public class Media
    {
        public string URL { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
