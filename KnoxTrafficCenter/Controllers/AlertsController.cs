using Microsoft.AspNetCore.Mvc;
using KnoxTrafficCenter.Services;
using System.Threading.Tasks;
using KnoxTrafficCenter.Models.TDOT;
using System.Collections.Generic;

namespace KnoxTrafficCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController(TDOTAPIService tdotApiService) : ControllerBase
    {
        [HttpGet("incidents")]
        public async Task<IActionResult> GetIncidents()
        {
            var incidents = await tdotApiService.GetIncidentsAsync();
            return Ok(incidents ?? new List<Event>());
        }

        [HttpGet("construction")]
        public async Task<IActionResult> GetConstruction()
        {
            var construction = await tdotApiService.GetConstructionAsync();
            return Ok(construction ?? new List<Event>());
        }

        [HttpGet("weather")]
        public async Task<IActionResult> GetWeather()
        {
            var weather = await tdotApiService.GetWeatherAsync();
            return Ok(weather ?? new List<Event>());
        }
    }
}
