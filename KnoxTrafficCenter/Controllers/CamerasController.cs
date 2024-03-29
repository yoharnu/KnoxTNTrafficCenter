using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KnoxTrafficCenter.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CamerasController(JsonFileCameraService cameraService) : ControllerBase
{
    // GET: api/<CamerasController>
    [HttpGet]
    public IEnumerable<Camera> Get()
    {
        return cameraService.GetCameras();
    }

    // GET api/<CamerasController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }
}
