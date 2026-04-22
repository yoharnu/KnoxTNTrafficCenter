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

    private List<CameraGroup>? cachedCameras;
    private DateTime lastCamerasRefresh = DateTime.MinValue;

    private List<Event>? cachedIncidents;
    private DateTime lastIncidentsRefresh = DateTime.MinValue;

    private List<Event>? cachedConstruction;
    private DateTime lastConstructionRefresh = DateTime.MinValue;

    private List<Event>? cachedWeather;
    private DateTime lastWeatherRefresh = DateTime.MinValue;

    private readonly TimeSpan dataRefreshInterval = TimeSpan.FromMinutes(2);

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

    private async Task<T?> GetFromTDOTApiAsync<T>(Func<TDOTAPI, string?> endpointSelector, string resourceName) where T : class, new()
    {
        api = await GetTDOTAPIAsync();

        if (api == null)
        {
            logger.LogError("TDOTAPI is null. Please check the configuration or API availability.");
            return null;
        }

        string? endpoint = endpointSelector(api);

        if (!string.IsNullOrEmpty(api.APIBaseURL) && !string.IsNullOrEmpty(api.APIKey) && !string.IsNullOrEmpty(endpoint))
        {
            using var httpClient = CreateHttpClient(api.APIBaseURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", api.APIKey);

            logger.LogDebug($"Using API Base URL: {api.APIBaseURL}");
            logger.LogDebug($"Requesting {resourceName} from: {endpoint}");

            var response = await httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await JsonSerializer.DeserializeAsync<T>(stream, options) ?? new T();
            }
        }

        logger.LogError($"Unable to retrieve {resourceName}. Please check the configuration or API availability.");
        return null;
    }

    private async Task<List<Event>> GetKnoxEventsAsync(Func<TDOTAPI, string?> endpointSelector, string resourceName)
    {
        var events = await GetFromTDOTApiAsync<List<Event>>(endpointSelector, resourceName) ?? [];
        return events.Where(x => x.Locations.Any(l => l.CountyName == "Knox")).ToList();
    }

    public async Task<List<CameraGroup>> GetCamerasAsync()
    {
        if (cachedCameras != null && DateTime.UtcNow - lastCamerasRefresh < dataRefreshInterval)
        {
            return cachedCameras;
        }

        var tdotCameras = await GetFromTDOTApiAsync<List<Models.TDOT.Camera>>(a => a.Cameras, "cameras");

        if (tdotCameras == null)
        {
            return null;
        }

        tdotCameras = tdotCameras.Where(x => x.Jurisdiction == "Knoxville" && x.Active == "true").OrderBy(x => x.Id).ToList();
        logger.LogDebug($"Retrieved {tdotCameras.Count} cameras from TDOT API.");
        logger.LogDebug($"Camera Routes: {string.Join(", ", tdotCameras.Select(x => x.Route).Distinct())}");
        var cameras = tdotCameras.Select(x => new Models.Camera(x)).Where(x => x.Road != "I-26" && x.Road != "I-81").OrderBy(x => x.Road).ThenBy(x => x.MM ?? float.MaxValue).ToList();
        var cameraGroups = cameras.ToLookup(x => x.Road);

        var i40Group = new CameraGroup("I-40");
        if (cameraGroups.Contains("I-40")) i40Group.AddRange(cameraGroups["I-40"]);
        var i640Group = new CameraGroup("I-640");
        if (cameraGroups.Contains("I-640")) i640Group.AddRange(cameraGroups["I-640"]);
        var i75Group = new CameraGroup("I-75");
        if (cameraGroups.Contains("I-75")) i75Group.AddRange(cameraGroups["I-75"]);
        var i275Group = new CameraGroup("I-275");
        if (cameraGroups.Contains("I-275")) i275Group.AddRange(cameraGroups["I-275"]);
        var i140Group = new CameraGroup("Pellissippi Parkway");
        if (cameraGroups.Contains("SR-162")) i140Group.AddRange(cameraGroups["SR-162"]);
        if (cameraGroups.Contains("I-140")) i140Group.AddRange(cameraGroups["I-140"]);
        var sr115Group = new CameraGroup("Alcoa Highway");
        if (cameraGroups.Contains("SR-115")) sr115Group.AddRange(cameraGroups["SR-115"]);

        var otherGroup = new CameraGroup("Other");
        var excludedRoads = new HashSet<string> { "I-40", "I-640", "I-75", "I-275", "I-140", "SR-162", "SR-115", "US-129" };
        foreach (var group in cameraGroups)
        {
            if (!excludedRoads.Contains(group.Key))
            {
                otherGroup.AddRange(group);
            }
        }

        cachedCameras = [i40Group, i640Group, i75Group, i275Group, i140Group, sr115Group, otherGroup];
        lastCamerasRefresh = DateTime.UtcNow;
        return cachedCameras;
    }

    public async Task<List<Event>> GetIncidentsAsync()
    {
        if (cachedIncidents != null && DateTime.UtcNow - lastIncidentsRefresh < dataRefreshInterval)
        {
            return cachedIncidents;
        }

        cachedIncidents = await GetKnoxEventsAsync(a => a.Incidents, "incidents");
        lastIncidentsRefresh = DateTime.UtcNow;
        return cachedIncidents;
    }

    public async Task<List<Event>> GetConstructionAsync()
    {
        if (cachedConstruction != null && DateTime.UtcNow - lastConstructionRefresh < dataRefreshInterval)
        {
            return cachedConstruction;
        }

        cachedConstruction = await GetKnoxEventsAsync(a => a.Construction, "construction events");
        lastConstructionRefresh = DateTime.UtcNow;
        return cachedConstruction;
    }

    public async Task<List<Event>> GetWeatherAsync()
    {
        if (cachedWeather != null && DateTime.UtcNow - lastWeatherRefresh < dataRefreshInterval)
        {
            return cachedWeather;
        }

        cachedWeather = await GetKnoxEventsAsync(a => a.Weather, "weather events");
        lastWeatherRefresh = DateTime.UtcNow;
        return cachedWeather;
    }
}
