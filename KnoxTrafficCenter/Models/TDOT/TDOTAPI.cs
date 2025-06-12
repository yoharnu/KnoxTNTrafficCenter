namespace KnoxTrafficCenter.Models.TDOT
{
    public class TDOTAPI
    {
        public string APIBaseURL { get; set; } = string.Empty;
        public string APIKey { get; set; } = string.Empty;
        public string Incidents { get; set; } = string.Empty;
        public string Construction { get; set; } = string.Empty;
        public string Cameras { get; set; } = string.Empty;
        public string MessageSigns { get; set; } = string.Empty;
        public string RestAreas { get; set; } = string.Empty;
        public string SpecialEvents { get; set; } = string.Empty;
        public string Weather { get; set; } = string.Empty;
        public string CountyWideWeather { get; set; } = string.Empty;

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
