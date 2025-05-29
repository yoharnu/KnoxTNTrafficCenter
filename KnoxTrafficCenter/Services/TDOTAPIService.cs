using KnoxTrafficCenter.Models.TDOT;
using System.Net.Http.Headers;
using System.Text.Json;

namespace KnoxTrafficCenter.Services;

public class TDOTAPIService
{
    private readonly HttpClient HttpClient;
    private ILogger<TDOTAPIService> logger;
    private TDOTAPI? api;

    public TDOTAPIService(ILogger<TDOTAPIService> logger)
    {
        this.logger = logger;

        HttpClient = new HttpClient
        {
            BaseAddress = new Uri("https://smartway.tn.gov/config/")
        };
        HttpClient.DefaultRequestHeaders.Accept.Clear();
        HttpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<TDOTAPI?> GetTDOTAPIAsync()
    {
        HttpResponseMessage response = await HttpClient.GetAsync("config.prod.json");
        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            api = JsonSerializer.Deserialize<TDOTAPI>(stream, options);
            if (api == null)
            {
                logger.LogError("Unable to parse config for TDOT API");
            }

            return api;
        }
        logger.LogError("Unable to get config for TDOT API");
        return null;
    }

    public async Task<List<Event>> GetIncidentsAsync()
    {
        if (api == null)
        {
            logger.LogError("TDOTAPI is null. Please check the configuration or API availability.");
            return [];
        }

        if (!string.IsNullOrEmpty(api.APIBaseURL) && !string.IsNullOrEmpty(api.APIKey) && !string.IsNullOrEmpty(api.Incidents))
        {
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(api.APIBaseURL)
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", api.APIKey);

            logger.LogDebug($"Using API Base URL: {api.APIBaseURL}");
            logger.LogDebug($"Using headers: {httpClient.DefaultRequestHeaders.ToString()}");
            logger.LogDebug($"Requesting incidents from: {api.Incidents}");

            var response = await httpClient.GetAsync(api.Incidents);
            logger.LogDebug(response.ToString());
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var incidents = await JsonSerializer.DeserializeAsync<List<Event>>(stream, options) ?? new();
                return incidents.Where(x => x.Locations.Select(x => x.CountyName).Contains("Knox")).ToList();
            }
        }
        logger.LogError("Unable to retrieve incidents. Please check the configuration or API availability.");
        return null;
    }

    public async Task<List<Models.Camera>> GetCamerasAsync()
    {
        if (api == null)
        {
            logger.LogError("TDOTAPI is null. Please check the configuration or API availability.");
            return [];
        }

        if (!string.IsNullOrEmpty(api.APIBaseURL) && !string.IsNullOrEmpty(api.APIKey) && !string.IsNullOrEmpty(api.Cameras))
        {
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(api.APIBaseURL)
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", api.APIKey);

            logger.LogDebug($"Using API Base URL: {api.APIBaseURL}");
            logger.LogDebug($"Using headers: {httpClient.DefaultRequestHeaders.ToString()}");
            logger.LogDebug($"Requesting cameras from: {api.Cameras}");

            var response = await httpClient.GetAsync(api.Cameras);
            logger.LogDebug(response.ToString());
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tdotCameras = await JsonSerializer.DeserializeAsync<List<Models.TDOT.Camera>>(stream, options) ?? new();
                tdotCameras = tdotCameras.Where(x => x.Jurisdiction == "Knoxville" && x.Active == "true").OrderBy(x => x.Id).ToList();
                return tdotCameras.Select(x => new Models.Camera(x)).ToList();
            }
        }
        logger.LogError("Unable to retrieve cameras. Please check the configuration or API availability.");
        return null;
    }
}
