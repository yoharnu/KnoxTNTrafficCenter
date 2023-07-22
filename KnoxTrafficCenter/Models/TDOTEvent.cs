using System.Text.Json.Serialization;

namespace KnoxTrafficCenter.Models
{
    public class TDOTEvent
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [JsonPropertyName("beginningDate")]
        public DateTime BeginDate { get; set; }
        [JsonPropertyName("endingDate")]
        public DateTime EndDate { get; set; }
        [JsonPropertyName("eventTypeName")]
        public string EventType { get; set; }
        public string Status { get; set; }
        public Location[] Locations { get; set; }

        public class Location
        {
            public string Type { get; set; }
            public int CountyId { get; set; }
            public string CountyName { get; set; }
        }
    }    
}
