using System.Text.Json.Serialization;

namespace KnoxTrafficCenter.Models
{
    public class Camera
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Road { get; set; } = "";
        public string Location { get; set; } = "";
        public float? MM { get; set; }
        public Media Video { get; set; } = new();
        public Media Image { get; set; } = new();

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
            Road = camera.Route;
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
            CleanData();
        }

        private void CleanData()
        {
            Road = Road.Trim();

            if (Road.Length == 0)
            {
                if (Title.Contains("I-40")) Road = "I-40";
                else if (Title.Contains("I-640")) Road = "I-640";
                else if (Title.Contains("I-75")) Road = "I-75";
                else if (Title.Contains("I-275")) Road = "I-275";
                else if (Title.Contains("I-81")) Road = "I-81";
                else if (Title.Contains("I-26")) Road = "I-26";
            }
            else
            {
                List<string> SRList = ["162", "115", "158"];
                if (SRList.Contains(Road))
                    Road = "SR-" + Road;
            }

            if (Road == "I-40" && MM == 1 && Title.Contains("Turkey Creek")) MM = 374;
            if (Road == "I-40/75" && MM == 0 && Title.Contains("Scales")) MM = 371.6f;
            if (Road == "I-40" && MM == 0 && Title.Contains("447.4")) MM = 447.4f;
            if (Road == "I-640" && MM == 0 && Title.Contains("Bruhin")) MM = 4.2f;

            if ((Road == "I-40/75" || Road == "I-75") && MM >= 368 && MM <= 385)
                Road = "I-40";
            if (Road == "I-75" && MM <= 3.4f)
                Road = "I-640";
            if (Road == "US-129")
                Road = "SR-115";
        }
    }

    public class Media
    {
        public string URL { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
