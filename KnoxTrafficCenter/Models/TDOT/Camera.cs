namespace KnoxTrafficCenter.Models.TDOT;
public class Camera
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string HttpsVideoUrl { get; set; } = string.Empty;
    public string Jurisdiction { get; set; } = string.Empty;
    public string Active { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string MileMarker { get; set; } = string.Empty;
}
