namespace KnoxTrafficCenter.Models.TDOT;

public class Location
{
    public string Type { get; set; }
    public int CountyId { get; set; }
    public string CountyName { get; set; }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
