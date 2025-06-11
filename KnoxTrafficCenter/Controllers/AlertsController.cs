using Microsoft.AspNetCore.Mvc;
using KnoxTrafficCenter.Services;
using System.Threading.Tasks;

namespace KnoxTrafficCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController(TDOTAPIService tdotApiService) : ControllerBase
    {
        [HttpGet("incidents")]
        public async Task<IActionResult> GetIncidents()
        {
            var api = await tdotApiService.GetTDOTAPIAsync();
            return Ok(api.Incidents);
        }

        [HttpGet("construction")]
        public async Task<IActionResult> GetConstruction()
        {
            var api = await tdotApiService.GetTDOTAPIAsync();
            return Ok(api.Construction);
        }

        [HttpGet("weather")]
        public async Task<IActionResult> GetWeather()
        {
            var api = await tdotApiService.GetTDOTAPIAsync();
            return Ok(api.Weather);
        }
    }
}
