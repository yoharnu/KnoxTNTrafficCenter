using System.Text.Json.Serialization;

namespace KnoxTrafficCenter.Models.TDOT;

public class Event
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Location[] Locations { get; set; } = Array.Empty<Location>();

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
