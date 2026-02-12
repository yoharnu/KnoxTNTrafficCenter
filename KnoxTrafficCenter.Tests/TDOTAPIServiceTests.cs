using KnoxTrafficCenter.Services;
using KnoxTrafficCenter.Models;
using KnoxTrafficCenter.Models.TDOT;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Reflection;
using System.Net;
using System.Text.Json;

namespace KnoxTrafficCenter.Tests;

public class TDOTAPIServiceTests
{
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public MockHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler(request);
        }
    }

    private class TestableTDOTAPIService : TDOTAPIService
    {
        private readonly HttpMessageHandler _handler;

        public TestableTDOTAPIService(ILogger<TDOTAPIService> logger, IConfiguration configuration, HttpMessageHandler handler)
            : base(logger, configuration)
        {
            _handler = handler;
        }

        protected override HttpClient CreateHttpClient(Uri baseAddress)
        {
            // We use the handler to mock responses
            return new HttpClient(_handler) { BaseAddress = baseAddress };
        }
    }

    [Fact]
    public async Task GetCamerasAsync_ReturnsCameras_WhenApiCallSucceeds()
    {
        // Arrange
        var mockLogger = NullLogger<TDOTAPIService>.Instance;
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

        var apiConfig = new TDOTAPI
        {
            APIBaseURL = "http://test.com/",
            APIKey = "key",
            Cameras = "cameras"
        };

        var cameras = new List<KnoxTrafficCenter.Models.TDOT.Camera>
        {
            new KnoxTrafficCenter.Models.TDOT.Camera {
                Id = 1,
                Jurisdiction = "Knoxville",
                Active = "true",
                Route = "I-40"
            }
        };

        var handler = new MockHttpMessageHandler(async request =>
        {
            if (request.RequestUri.ToString() == "http://test.com/cameras")
            {
                var json = JsonSerializer.Serialize(cameras);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        var service = new TestableTDOTAPIService(mockLogger, mockConfig.Object, handler);

        // Use reflection to set private api field
        var apiField = typeof(TDOTAPIService).GetField("api", BindingFlags.NonPublic | BindingFlags.Instance);
        if (apiField == null) throw new InvalidOperationException("Field 'api' not found");
        apiField.SetValue(service, apiConfig);

        // Set lastConfigRefresh to avoid config refresh call
        var refreshField = typeof(TDOTAPIService).GetField("lastConfigRefresh", BindingFlags.NonPublic | BindingFlags.Instance);
        if (refreshField == null) throw new InvalidOperationException("Field 'lastConfigRefresh' not found");
        refreshField.SetValue(service, DateTime.UtcNow);

        // Act
        var result = await service.GetCamerasAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var i40Group = result.FirstOrDefault(g => g.Title == "I-40");
        Assert.NotNull(i40Group);
        Assert.Single(i40Group.Cameras);
    }
}
