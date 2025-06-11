using Microsoft.AspNetCore.Mvc;
using KnoxTrafficCenter.Services;
using System.Threading.Tasks;

namespace KnoxTrafficCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController(TDOTAPIService tdotApiService) : ControllerBase
    {
        private readonly TDOTAPIService _tdotApiService = tdotApiService;

        [HttpGet("incidents")]
        public async Task<IActionResult> GetIncidents()
        {
            var api = await _tdotApiService.GetTDOTAPIAsync();
            return Ok(api.Incidents);
        }

        [HttpGet("construction")]
        public async Task<IActionResult> GetConstruction()
        {
            var api = await _tdotApiService.GetTDOTAPIAsync();
            return Ok(api.Construction);
        }

        [HttpGet("weather")]
        public async Task<IActionResult> GetWeather()
        {
            var api = await _tdotApiService.GetTDOTAPIAsync();
            return Ok(api.Weather);
        }
    }
}
