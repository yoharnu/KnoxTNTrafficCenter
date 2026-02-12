using System.Runtime.CompilerServices;

namespace KnoxTrafficCenter.Models;

public class CameraGroup(string title)
{
    private readonly HashSet<int> _cameraIds = [];

    public string Title { get; set; } = title;
    public List<Camera> Cameras { get; set; } = new List<Camera>();
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize<CameraGroup>(this);
    }

    public void Add(Camera camera)
    {
        if (_cameraIds.Add(camera.Id))
        {
            Cameras.Add(camera);
        }
    }

    public void AddRange(IEnumerable<Camera> cameras)
    {
        foreach (var camera in cameras)
        {
            Add(camera);
        }
    }
}
