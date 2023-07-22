namespace KnoxTrafficCenter.Models
{
    public class TDOTAPI
    {
        public string APIBaseURL { get; set; }
        public string APIKey { get; set; }
        public string Incidents { get; set; }
        public string Construction { get; set; }
        public string Cameras { get; set; }
        public string MessageSigns { get; set; }
        public string RestAreas { get; set; }
        public string SpecialEvents { get; set; }
        public string Weather { get; set; }
        public string CountyWideWeather { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize<TDOTAPI>(this);
        }
    }
}
