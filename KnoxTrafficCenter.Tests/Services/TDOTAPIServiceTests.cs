using FluentAssertions;
using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Models.TDOT;
using KnoxTrafficCenter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace KnoxTrafficCenter.Tests.Services;

public class TDOTAPIServiceTests
{
    private readonly Mock<ILogger<TDOTAPIService>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpHandlerMock;
    private readonly TestableTDOTAPIService _service;

    public TDOTAPIServiceTests()
    {
        _loggerMock = new Mock<ILogger<TDOTAPIService>>();
        _httpHandlerMock = new Mock<HttpMessageHandler>();

        var inMemorySettings = new Dictionary<string, string> {
            {"TDOTApi:BaseUrl", "https://smartway.tn.gov/traffic/"},
            {"TDOTApi:ConfigRefreshIntervalHours", "1"}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _service = new TestableTDOTAPIService(_loggerMock.Object, configuration, _httpHandlerMock.Object);
    }

    [Fact]
    public async Task GetTDOTAPIAsync_ReturnsConfig_WhenSuccess()
    {
        // Arrange
        var tdotApi = new TDOTAPI
        {
            APIBaseURL = "https://api.tdot.com",
            APIKey = "key123",
            Cameras = "cameras",
            Incidents = "incidents"
        };
        var json = JsonSerializer.Serialize(tdotApi);

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith("config.prod.json")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        // Act
        var result = await _service.GetTDOTAPIAsync();

        // Assert
        result.Should().NotBeNull();
        result!.APIBaseURL.Should().Be("https://api.tdot.com");
    }

    [Fact]
    public async Task GetTDOTAPIAsync_ReturnsNull_WhenApiFails()
    {
        // Arrange
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        // Act
        var result = await _service.GetTDOTAPIAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCamerasAsync_ReturnsGroups_WhenSuccess()
    {
        // Arrange
        var tdotApi = new TDOTAPI
        {
            APIBaseURL = "https://api.tdot.com/",
            APIKey = "key123",
            Cameras = "cameras"
        };

        SetupConfigResponse(tdotApi);

        var cameras = new List<KnoxTrafficCenter.Models.TDOT.Camera>
        {
            new() { Id = 1, Jurisdiction = "Knoxville", Active = "true", Route = "I-40", MileMarker = "380" },
            new() { Id = 2, Jurisdiction = "Knoxville", Active = "true", Route = "I-75", MileMarker = "100" },
            new() { Id = 3, Jurisdiction = "Nashville", Active = "true", Route = "I-40" } // Should be filtered out
        };
        var camerasJson = JsonSerializer.Serialize(cameras);

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString() == "https://api.tdot.com/cameras"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(camerasJson)
            });

        // Act
        var result = await _service.GetCamerasAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(7); // 7 fixed groups

        var i40Group = result.First(g => g.Title == "I-40");
        i40Group.Cameras.Should().HaveCount(1);
        i40Group.Cameras.First().Id.Should().Be(1);

        var i75Group = result.First(g => g.Title == "I-75");
        i75Group.Cameras.Should().HaveCount(1);
        i75Group.Cameras.First().Id.Should().Be(2);
    }

    [Fact]
    public async Task GetIncidentsAsync_ReturnsKnoxIncidents()
    {
        // Arrange
        var tdotApi = new TDOTAPI
        {
            APIBaseURL = "https://api.tdot.com/",
            APIKey = "key123",
            Incidents = "incidents"
        };
        SetupConfigResponse(tdotApi);

        var incidents = new List<Event>
        {
            new() { Id = 1, Locations = [new Location { CountyName = "Knox" }] },
            new() { Id = 2, Locations = [new Location { CountyName = "Davidson" }] }
        };

        var incidentsJson = JsonSerializer.Serialize(incidents);

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString() == "https://api.tdot.com/incidents"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(incidentsJson)
            });

        // Act
        var result = await _service.GetIncidentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetConstructionAsync_ReturnsEmptyList_WhenApiConfigIsNull()
    {
        // Arrange
        // Simulate a failure to get the config
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith("config.prod.json")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        // Act
        var result = await _service.GetConstructionAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConstructionAsync_ReturnsEmptyList_WhenApiFails()
    {
        // Arrange
        var tdotApi = new TDOTAPI
        {
            APIBaseURL = "https://api.tdot.com/",
            APIKey = "key123",
            Construction = "construction"
        };
        SetupConfigResponse(tdotApi);

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString() == "https://api.tdot.com/construction"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        // Act
        var result = await _service.GetConstructionAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConstructionAsync_ReturnsKnoxConstruction()
    {
        // Arrange
        var tdotApi = new TDOTAPI
        {
            APIBaseURL = "https://api.tdot.com/",
            APIKey = "key123",
            Construction = "construction"
        };
        SetupConfigResponse(tdotApi);

        var constructionEvents = new List<Event>
        {
            new() { Id = 1, Locations = [new Location { CountyName = "Knox" }] },
            new() { Id = 2, Locations = [new Location { CountyName = "Davidson" }] }
        };

        var constructionJson = JsonSerializer.Serialize(constructionEvents);

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString() == "https://api.tdot.com/construction"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(constructionJson)
            });

        // Act
        var result = await _service.GetConstructionAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetWeatherAsync_ReturnsKnoxWeather()
    {
        // Arrange
        var tdotApi = new TDOTAPI
        {
            APIBaseURL = "https://api.tdot.com/",
            APIKey = "key123",
            Weather = "weather"
        };
        SetupConfigResponse(tdotApi);

        var weatherEvents = new List<Event>
        {
            new() { Id = 1, Locations = [new Location { CountyName = "Knox" }] },
            new() { Id = 2, Locations = [new Location { CountyName = "Davidson" }] }
        };

        var weatherJson = JsonSerializer.Serialize(weatherEvents);

        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString() == "https://api.tdot.com/weather"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(weatherJson)
            });

        // Act
        var result = await _service.GetWeatherAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
    }

    private void SetupConfigResponse(TDOTAPI api)
    {
        var json = JsonSerializer.Serialize(api);
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith("config.prod.json")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });
    }

    // Subclass for testing
    public class TestableTDOTAPIService : TDOTAPIService
    {
        private readonly HttpMessageHandler _handler;

        public TestableTDOTAPIService(ILogger<TDOTAPIService> logger, IConfiguration configuration, HttpMessageHandler handler)
            : base(logger, configuration)
        {
            _handler = handler;
            var baseUrl = configuration.GetValue<string>("TDOTApi:BaseUrl");
            // Re-initialize _httpClient to use the mock handler
            _httpClient = new HttpClient(_handler) { BaseAddress = new Uri(baseUrl!) };
        }

        protected override HttpClient CreateHttpClient(string? baseAddress = null)
        {
            var client = new HttpClient(_handler, false); // Don't dispose handler
            if (baseAddress != null)
            {
                client.BaseAddress = new Uri(baseAddress);
            }
            return client;
        }
    }
}
