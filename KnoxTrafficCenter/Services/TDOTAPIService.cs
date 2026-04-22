using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Models.TDOT;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace KnoxTrafficCenter.Services;

public class TDOTAPIService
{
    protected HttpClient _httpClient;
    private ILogger<TDOTAPIService> logger;
    private TDOTAPI? api;
    private DateTime lastConfigRefresh = DateTime.MinValue;
    private readonly TimeSpan configRefreshInterval = TimeSpan.FromHours(24);

    public TDOTAPIService(ILogger<TDOTAPIService> logger, IConfiguration configuration)
    {
        this.logger = logger;

        string baseUrl = configuration.GetValue<string>("TDOTApi:BaseUrl") ?? throw new ArgumentNullException("TDOTApi:BaseUrl", "TDOT API Base URL must be configured.");
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        
        // Read config refresh interval from appsettings.json or use default
        int refreshHours = configuration.GetValue<int>("TDOTApi:ConfigRefreshIntervalHours", 24);
        configRefreshInterval = TimeSpan.FromHours(refreshHours);
        logger.LogInformation($"TDOT API configuration will refresh every {refreshHours} hours");
    }

    protected virtual HttpClient CreateHttpClient(string? baseAddress = null)
    {
        var client = new HttpClient();
        if (baseAddress != null)
        {
            client.BaseAddress = new Uri(baseAddress);
        }
        return client;
    }

    public async Task<TDOTAPI?> GetTDOTAPIAsync()
    {
        // Check if we already have a valid configuration and it's not time to refresh yet
        if (api != null && DateTime.UtcNow - lastConfigRefresh < configRefreshInterval)
        {
            logger.LogDebug("Using cached TDOT API configuration");
            return api;
        }

        logger.LogInformation("Refreshing TDOT API configuration");
        HttpResponseMessage response = await _httpClient.GetAsync("config.prod.json");
        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            api = JsonSerializer.Deserialize<TDOTAPI>(stream, options);
            if (api == null)
            {
                logger.LogError("Unable to parse config for TDOT API");
            }
            else
            {
                // Update the refresh timestamp
                lastConfigRefresh = DateTime.UtcNow;
                logger.LogInformation($"Successfully refreshed TDOT API configuration. Next refresh at: {lastConfigRefresh + configRefreshInterval}");
            }

            return api;
        }
        logger.LogError("Unable to get config for TDOT API");
        
        // If refresh failed but we have an existing config, return it
        if (api != null)
        {
            logger.LogWarning("Using stale TDOT API configuration because refresh failed");
            return api;
        }
        
        return null;
    }

    public async Task<List<CameraGroup>> GetCamerasAsync()
    {
        api = await GetTDOTAPIAsync();
        
        if (api == null)
        {
            logger.LogError("TDOTAPI is null. Please check the configuration or API availability.");
            return [];
        }

        if (!string.IsNullOrEmpty(api.APIBaseURL) && !string.IsNullOrEmpty(api.APIKey) && !string.IsNullOrEmpty(api.Cameras))
        {
            using var httpClient = CreateHttpClient(api.APIBaseURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", api.APIKey);

            logger.LogDebug($"Using API Base URL: {api.APIBaseURL}");
            logger.LogDebug($"Requesting cameras from: {api.Cameras}");

            var response = await httpClient.GetAsync(api.Cameras);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tdotCameras = await JsonSerializer.DeserializeAsync<List<Models.TDOT.Camera>>(stream, options) ?? new();
                tdotCameras = tdotCameras.Where(x => x.Jurisdiction == "Knoxville" && x.Active == "true").OrderBy(x => x.Id).ToList();
                logger.LogDebug($"Retrieved {tdotCameras.Count} cameras from TDOT API.");
                logger.LogDebug($"Camera Routes: {string.Join(", ", tdotCameras.Select(x => x.Route).Distinct())}");
                var cameras = tdotCameras.Select(x => new Models.Camera(x))
                    .Where(x => x.Road != "I-26" && x.Road != "I-81")
                    .OrderBy(x => x.Road)
                    .ThenBy(x => x.MM ?? float.MaxValue);

                var i40Group = new CameraGroup("I-40");
                var i640Group = new CameraGroup("I-640");
                var i75Group = new CameraGroup("I-75");
                var i275Group = new CameraGroup("I-275");
                var i140Group = new CameraGroup("Pellissippi Parkway");
                var sr115Group = new CameraGroup("Alcoa Highway");
                var otherGroup = new CameraGroup("Other");

                var roadToGroup = new Dictionary<string, CameraGroup>(StringComparer.OrdinalIgnoreCase)
                {
                    { "I-40", i40Group },
                    { "I-640", i640Group },
                    { "I-75", i75Group },
                    { "I-275", i275Group },
                    { "SR-115", sr115Group }
                };

                // Create a list to preserve ordering if needed, or handle specifically
                // In original code, Pellissippi Parkway (i140Group) was SR-162 then I-140
                // US-129 was excluded from Other but not added elsewhere.
                var cameraList = cameras.ToList();

                foreach (var camera in cameraList.Where(x => x.Road == "SR-162")) i140Group.Add(camera);
                foreach (var camera in cameraList.Where(x => x.Road == "I-140")) i140Group.Add(camera);

                foreach (var camera in cameraList)
                {
                    if (string.IsNullOrEmpty(camera.Road))
                    {
                        otherGroup.Add(camera);
                        continue;
                    }

                    if (camera.Road == "SR-162" || camera.Road == "I-140") continue; // Already handled

                    if (roadToGroup.TryGetValue(camera.Road, out var group))
                    {
                        group.Add(camera);
                    }
                    else if (camera.Road != "US-129") // Original excludedRoads included US-129 but didn't add it to any group
                    {
                        otherGroup.Add(camera);
                    }
                }

                return [i40Group, i640Group, i75Group, i275Group, i140Group, sr115Group, otherGroup];
            }
        }
        logger.LogError("Unable to retrieve cameras. Please check the configuration or API availability.");
        return null;
    }

    public async Task<List<Event>> GetIncidentsAsync()
    {
        api = await GetTDOTAPIAsync();
        
        if (api == null)
        {
            logger.LogError("TDOTAPI is null. Please check the configuration or API availability.");
            return [];
        }

        if (!string.IsNullOrEmpty(api.APIBaseURL) && !string.IsNullOrEmpty(api.APIKey) && !string.IsNullOrEmpty(api.Incidents))
        {
            using var httpClient = CreateHttpClient(api.APIBaseURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", api.APIKey);

            logger.LogDebug($"Using API Base URL: {api.APIBaseURL}");
            logger.LogDebug($"Requesting incidents from: {api.Incidents}");

            var response = await httpClient.GetAsync(api.Incidents);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var incidents = await JsonSerializer.DeserializeAsync<List<Event>>(stream, options) ?? new();
                return incidents.Where(x => x.Locations.Select(x => x.CountyName).Contains("Knox")).ToList();
            }
        }
        logger.LogError("Unable to retrieve incidents. Please check the configuration or API availability.");
        return [];
    }

    public async Task<List<Event>> GetConstructionAsync()
    {
        api = await GetTDOTAPIAsync();

        if (api == null)
        {
            logger.LogError("TDOTAPI is null. Please check the configuration or API availability.");
            return [];
        }

        if (!string.IsNullOrEmpty(api.APIBaseURL) && !string.IsNullOrEmpty(api.APIKey) && !string.IsNullOrEmpty(api.Construction))
        {
            using var httpClient = CreateHttpClient(api.APIBaseURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", api.APIKey);

            logger.LogDebug($"Using API Base URL: {api.APIBaseURL}");
            logger.LogDebug($"Requesting construction events from: {api.Construction}");

            var response = await httpClient.GetAsync(api.Construction);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var constructionEvents = await JsonSerializer.DeserializeAsync<List<Event>>(stream, options) ?? new();
                return constructionEvents.Where(x => x.Locations.Select(x => x.CountyName).Contains("Knox")).ToList();
            }
        }
        logger.LogError("Unable to retrieve construction events. Please check the configuration or API availability.");
        return [];
    }

    public async Task<List<Event>> GetWeatherAsync()
    {
        api = await GetTDOTAPIAsync();

        if (api == null)
        {
            logger.LogError("TDOTAPI is null. Please check the configuration or API availability.");
            return [];
        }

        if (!string.IsNullOrEmpty(api.APIBaseURL) && !string.IsNullOrEmpty(api.APIKey) && !string.IsNullOrEmpty(api.Weather))
        {
            using var httpClient = CreateHttpClient(api.APIBaseURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", api.APIKey);

            logger.LogDebug($"Using API Base URL: {api.APIBaseURL}");
            logger.LogDebug($"Requesting weather events from: {api.Weather}");

            var response = await httpClient.GetAsync(api.Weather);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var weatherEvents = await JsonSerializer.DeserializeAsync<List<Event>>(stream, options) ?? [];
                return weatherEvents.Where(x => x.Locations.Any(l => l.CountyName == "Knox")).ToList();
            }
        }
        logger.LogError("Unable to retrieve weather events. Please check the configuration or API availability.");
        return [];
    }
}
