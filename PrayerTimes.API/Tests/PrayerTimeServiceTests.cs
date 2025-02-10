using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using PrayerTimes.API.Models;
using PrayerTimes.API.Services;

namespace PrayerTimes.API.Tests
{
    public class PrayerTimeServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly PrayerTimeService _service;
        private readonly ILogger<PrayerTimeService> _logger;

        public PrayerTimeServiceTests()
        {
            // Mock HttpMessageHandler to fake API calls
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            // Use mocked handler inside HttpClient
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.aladhan.com/v1") // Fake API URL
            };

            // Mock logger
            _logger = new LoggerFactory().CreateLogger<PrayerTimeService>();

            // Initialize the service with mocked dependencies
            _service = new PrayerTimeService(_httpClient, _logger);
        }

        [Fact]
        public async Task GetDailyPrayerTimesAsync_ShouldReturnPrayerTimes_WhenApiCallIsSuccessful()
        {
            // Arrange: Mock API response
            var fakeResponse = @"{
                ""data"": { 
                    ""timings"": { ""Fajr"": ""05:30"", ""Dhuhr"": ""12:45"" }
                }
            }";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeResponse)
                });

            // Act: Call the service method
            var result = await _service.GetDailyPrayerTimesAsync("Bergen", "Norway");

            // Assert: Check results
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Fajr.Should().Be("05:30");
            result.Data.Dhuhr.Should().Be("12:45");
        }

        [Fact]
        public async Task GetDailyPrayerTimesAsync_ShouldReturnError_WhenApiFails()
        {
            // Arrange: Simulate a failed API call
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act
            var result = await _service.GetDailyPrayerTimesAsync("Bergen", "Norway");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNull();
        }

        [Fact]
        public async Task GetDailyPrayerTimesByCoordinatesAsync_ShouldReturnPrayerTimes_WhenApiCallIsSuccessful()
        {
            // Arrange
            var fakeResponse = @"{
                ""data"": { 
                    ""timings"": { ""Fajr"": ""05:30"", ""Dhuhr"": ""12:45"" }
                }
            }";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeResponse)
                });

            // Act
            var result = await _service.GetDailyPrayerTimesByCoordinatesAsync(60.39, 5.32);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Fajr.Should().Be("05:30");
            result.Data.Dhuhr.Should().Be("12:45");
        }

        [Fact]
        public async Task GetDailyPrayerTimesByCoordinatesAsync_ShouldReturnError_WhenApiFails()
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var result = await _service.GetDailyPrayerTimesByCoordinatesAsync(60.39, 5.32);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNull();
        }

        [Fact]
        public async Task GetDailyPrayerTimesAsync_ShouldThrowException_WhenCityIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetDailyPrayerTimesAsync("", "Norway"));
        }

        [Fact]
        public async Task GetDailyPrayerTimesAsync_ShouldThrowException_WhenCountryIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetDailyPrayerTimesAsync("Bergen", ""));
        }

        [Fact]
        public async Task GetDailyPrayerTimesByCoordinatesAsync_ShouldThrowException_WhenLatitudeIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetDailyPrayerTimesByCoordinatesAsync(100, 5.32));
        }

        [Fact]
        public async Task GetDailyPrayerTimesByCoordinatesAsync_ShouldThrowException_WhenLongitudeIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetDailyPrayerTimesByCoordinatesAsync(60.39, 200));
        }
    }
}
