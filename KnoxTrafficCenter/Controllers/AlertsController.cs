using Microsoft.AspNetCore.Mvc;
using KnoxTrafficCenter.Services;
using System.Threading.Tasks;

namespace KnoxTrafficCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController(TDOTAPIService tdotApiService, TDOTEventService tdotEventService) : ControllerBase
    {
        private readonly TDOTAPIService _tdotApiService = tdotApiService;
        private readonly TDOTEventService _tdotEventService = tdotEventService;

        [HttpGet("incidents")]
        public async Task<IActionResult> GetIncidents()
        {
            var incidents = await _tdotEventService.GetIncidentsAsync();
            return Ok(incidents);
        }

        [HttpGet("construction")]
        public async Task<IActionResult> GetConstruction()
        {
            var construction = await _tdotEventService.GetConstructionAsync();
            return Ok(construction);
        }

        [HttpGet("weather")]
        public async Task<IActionResult> GetWeather()
        {
            var weather = await _tdotEventService.GetWeatherAsync();
            return Ok(weather);
        }
    }
}
