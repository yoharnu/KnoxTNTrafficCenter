using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Models.TDOT;
using KnoxTrafficCenter.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace KnoxTrafficCenter.Pages;

public class IndexModel(ILogger<IndexModel> logger, JsonFileCameraService cameraService, TDOTAPIService tdotApiService) : PageModel
{
    public IEnumerable<Camera> Cameras { get; private set; } = [];
    public TDOTAPI? TDOTAPI { get; private set; } = new TDOTAPI();

    public List<Event> Incidents { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Cameras = cameraService.GetCameras();
        TDOTAPI = await tdotApiService.GetTDOTAPIAsync();
        logger.LogDebug($"TDOTAPI object: {TDOTAPI}");
        if (TDOTAPI != null && !string.IsNullOrEmpty(TDOTAPI.Incidents) && !string.IsNullOrEmpty(TDOTAPI.APIBaseURL) && !string.IsNullOrEmpty(TDOTAPI.APIKey))
        {
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(TDOTAPI.APIBaseURL)
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", TDOTAPI.APIKey);

            logger.LogDebug($"Using API Base URL: {TDOTAPI.APIBaseURL}");
            logger.LogDebug($"Using headers: {httpClient.DefaultRequestHeaders.ToString()}");
            logger.LogDebug($"Requesting incidents from: {TDOTAPI.Incidents}");

            var response = await httpClient.GetAsync(TDOTAPI.Incidents);
            logger.LogDebug(response.ToString());
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Incidents = await JsonSerializer.DeserializeAsync<List<Event>>(stream, options) ?? new();
            }
        }
    }
}