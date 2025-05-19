using KnoxTrafficCenter.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace KnoxTrafficCenter.Services
{
    public class TDOTAPIService
    {
        public TDOTAPIService()
        {
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://smartway.tn.gov/config/")
            };
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            FetchAPI();
        }

        private HttpClient HttpClient { get; set; }

        internal TDOTAPI? TDOTAPI { get; set; }

        private async void FetchAPI()
        {
            HttpResponseMessage response = await HttpClient.GetAsync("config.prod.json");
            if (response.IsSuccessStatusCode)
            {
                TDOTAPI = JsonSerializer.Deserialize<TDOTAPI>(await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
    }
}
