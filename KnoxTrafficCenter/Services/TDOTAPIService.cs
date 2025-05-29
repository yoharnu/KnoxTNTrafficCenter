using KnoxTrafficCenter.Models.TDOT;
using System.Net.Http.Headers;
using System.Text.Json;

namespace KnoxTrafficCenter.Services;

public class TDOTAPIService
{
    private readonly HttpClient HttpClient;

    public TDOTAPIService()
    {
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
            return JsonSerializer.Deserialize<TDOTAPI>(stream, options);
        }
        return null;
    }
}
