using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
        var client = new HttpClient(handler);
        int iterations = 10;

        Console.WriteLine("Warming up...");
        try {
            await client.GetAsync("http://localhost:5260/api/alerts/incidents");
            await client.GetAsync("http://localhost:5260/api/alerts/weather");
        } catch {}

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            await client.GetAsync("http://localhost:5260/api/alerts/incidents");
        }
        sw.Stop();

        Console.WriteLine($"Incidents: {sw.ElapsedMilliseconds} ms for {iterations} requests");

        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            await client.GetAsync("http://localhost:5260/api/alerts/weather");
        }
        sw.Stop();

        Console.WriteLine($"Weather: {sw.ElapsedMilliseconds} ms for {iterations} requests");
    }
}
