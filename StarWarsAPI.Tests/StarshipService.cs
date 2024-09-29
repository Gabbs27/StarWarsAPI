using Moq;
using Xunit;
using System.Net.Http;
using StarWarsAPI.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Text.Json;
using System.Collections.Generic;
using StarWarsAPI.Models;
using System.Net.Http.Json;
using Moq.Protected;

namespace StarWarsAPIChallenge.Tests
{
    public class StarshipServiceTests
    {
        private readonly StarshipService _starshipService;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<ILogger<StarshipService>> _loggerMock;

        public StarshipServiceTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<ILogger<StarshipService>>();

            _starshipService = new StarshipService(_httpClientFactoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetStarshipsAsync_ReturnsStarships_WhenApiResponseIsValid()
        {
            // Arrange
            var expectedStarships = new StarshipResponse
            {
                Results = new List<Starship>
                {
                    new Starship { Name = "Millennium Falcon", Manufacturer = "Corellian Engineering Corporation" }
                }
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(expectedStarships)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _starshipService.GetStarshipsAsync(null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Millennium Falcon", result[0].Name);
        }

        [Fact]
        public async Task GetStarshipsAsync_ReturnsNoStarships_WhenApiResponseIsEmpty()
        {
            // Arrange
            var expectedStarships = new StarshipResponse
            {
                Results = new List<Starship>() // Empty list
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(expectedStarships)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _starshipService.GetStarshipsAsync(null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetStarshipsAsync_FiltersStarshipsByManufacturer()
        {
            // Arrange
            var expectedStarships = new StarshipResponse
            {
                Results = new List<Starship>
                {
                    new Starship { Name = "Millennium Falcon", Manufacturer = "Corellian Engineering Corporation" },
                    new Starship { Name = "Star Destroyer", Manufacturer = "Kuat Drive Yards" }
                }
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(expectedStarships)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _starshipService.GetStarshipsAsync("Corellian Engineering Corporation");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Millennium Falcon", result[0].Name);
        }

        [Fact]
        public async Task GetStarshipsAsync_ThrowsException_WhenApiResponseIsError()
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
                    StatusCode = HttpStatusCode.InternalServerError // Simulate server error
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _starshipService.GetStarshipsAsync(null));
        }

        [Fact]
        public async Task GetStarshipsAsync_ReturnsEmpty_WhenNoManufacturerMatches()
        {
            // Arrange
            var expectedStarships = new StarshipResponse
            {
                Results = new List<Starship>
                {
                    new Starship { Name = "Millennium Falcon", Manufacturer = "Corellian Engineering Corporation" }
                }
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(expectedStarships)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _starshipService.GetStarshipsAsync("Nonexistent Manufacturer");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
