using KnoxTrafficCenter.Models;
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

    public async Task<List<CameraGroup>> GetCamerasAsync()
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
                logger.LogDebug($"Retrieved {tdotCameras.Count} cameras from TDOT API.");
                logger.LogDebug($"Camera Routes: {string.Join(", ", tdotCameras.Select(x => x.Route).Distinct())}");
                var cameras = tdotCameras.Select(x => new Models.Camera(x)).Where(x => x.Road != "I-26" && x.Road != "I-81").OrderBy(x => x.Road).ThenBy(x => x.MM ?? float.MaxValue).ToList();

                var i40Group = new CameraGroup("I-40");
                i40Group.AddRange(cameras.Where(x => x.Road == "I-40"));
                var i640Group = new CameraGroup("I-640");
                i640Group.AddRange(cameras.Where(x => x.Road == "I-640"));
                var i75Group = new CameraGroup("I-75");
                i75Group.AddRange(cameras.Where(x => x.Road == "I-75"));
                var i275Group = new CameraGroup("I-275");
                i275Group.AddRange(cameras.Where(x => x.Road == "I-275"));
                var i140Group = new CameraGroup("Pellissippi Parkway");
                i140Group.AddRange(cameras.Where(x => x.Road == "SR-162"));
                i140Group.AddRange(cameras.Where(x => x.Road == "I-140"));
                var sr115Group = new CameraGroup("Alcoa Highway");
                sr115Group.AddRange(cameras.Where(x => x.Road == "SR-115"));
                var otherGroup = new CameraGroup("Other");
                otherGroup.AddRange(cameras.Where(x => x.Road != "I-40" && x.Road != "I-640" && x.Road != "I-75" && x.Road != "I-275" && x.Road != "I-140" && x.Road != "SR-162" && x.Road != "SR-115" && x.Road != "US-129"));

                return [i40Group, i640Group, i75Group, i275Group, i140Group, sr115Group, otherGroup];
            }
        }
        logger.LogError("Unable to retrieve cameras. Please check the configuration or API availability.");
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

    public async Task<List<Event>> GetConstructionAsync()
    {

        if (api == null)
        {
            logger.LogError("TDOTAPI is null. Please check the configuration or API availability.");
            return [];
        }

        if (!string.IsNullOrEmpty(api.APIBaseURL) && !string.IsNullOrEmpty(api.APIKey) && !string.IsNullOrEmpty(api.Construction))
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
            logger.LogDebug($"Requesting construction events from: {api.Construction}");

            var response = await httpClient.GetAsync(api.Construction);
            logger.LogDebug(response.ToString());
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var constructionEvents = await JsonSerializer.DeserializeAsync<List<Event>>(stream, options) ?? new();
                return constructionEvents.Where(x => x.Locations.Select(x => x.CountyName).Contains("Knox")).ToList();
            }
        }
        logger.LogError("Unable to retrieve construction events. Please check the configuration or API availability.");
        return null;
    }
}
