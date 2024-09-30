using Xunit;
using Moq;
using System.Net.Http;
using StarWarsAPI.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using StarWarsAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq.Protected;
using Newtonsoft.Json;

namespace StarWarsAPI.Tests
{
    public class StarshipServiceTests
    {

        private readonly StarshipService _starshipService;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<ILogger<StarshipService>> _loggerMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;

        public StarshipServiceTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<ILogger<StarshipService>>();
            _memoryCacheMock = new Mock<IMemoryCache>();

            _starshipService = new StarshipService(_httpClientFactoryMock.Object, _loggerMock.Object, _memoryCacheMock.Object);
        }

        [Fact]
        public async Task GetStarshipsAsync_ReturnsCachedStarships_WhenCacheIsAvailable()
        {
            // Arrange
            var cachedStarships = new List<Starship>
            {
                new Starship { Name = "Millennium Falcon", Manufacturer = "Corellian Engineering Corporation" }
            };

            object outValue = cachedStarships;
            _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out outValue)).Returns(true);

            // Act
            var result = await _starshipService.GetStarshipsAsync(null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Millennium Falcon", result[0].Name);

            // Verify that HTTP request was not made
            _httpClientFactoryMock.Verify(x => x.CreateClient(It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async Task GetStarshipsAsync_ThrowsHttpRequestException_WhenApiResponseIsError()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var logMessages = new List<string>();
            _loggerMock.Setup(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>())
            ).Callback(new InvocationAction(invocation =>
            {
                var state = invocation.Arguments[2];  // This captures the log message (state)
                logMessages.Add(state.ToString());
            }));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _starshipService.GetStarshipsAsync(null));

            // Verify that logger was called and captured the correct log message
            Assert.Contains("HTTP request error occurred while fetching starships", logMessages);
        }
    }
}
