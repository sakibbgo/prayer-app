using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PrayerTimes.API.Models;

namespace PrayerTimes.API.Services
{
    public class PrayerTimeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PrayerTimeService> _logger;
        private const string BaseUrl = "https://api.aladhan.com/v1";

        public PrayerTimeService(HttpClient httpClient, ILogger<PrayerTimeService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponse<PrayerTimings>> GetDailyPrayerTimesAsync(string city, string country)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.", nameof(city));

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be empty.", nameof(country));

            try
            {
                var apiUrl = $"{BaseUrl}/timingsByCity?city={city}&country={country}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API call failed: {response.StatusCode} - {response.ReasonPhrase}");
                    return ApiResponse<PrayerTimings>.Fail($"Failed to fetch prayer times. Status Code: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var prayerData = JsonSerializer.Deserialize<DailyPrayerTimeResponse>(content);

                if (prayerData?.Data?.Timings == null)
                {
                    _logger.LogError("API response format is invalid.");
                    return ApiResponse<PrayerTimings>.Fail("Invalid response from the API.");
                }

                return ApiResponse<PrayerTimings>.Success(prayerData.Data.Timings);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Network error while fetching prayer times: {ex.Message}");
                return ApiResponse<PrayerTimings>.Fail("Network error. Please check your connection.");
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("API request timed out.");
                return ApiResponse<PrayerTimings>.Fail("Request timed out. Try again later.");
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error parsing API response: {ex.Message}");
                return ApiResponse<PrayerTimings>.Fail("Error processing server response.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return ApiResponse<PrayerTimings>.Fail("An unexpected error occurred. Please try again.");
            }
        }
        public async Task<ApiResponse<PrayerTimings>> GetDailyPrayerTimesByCoordinatesAsync(double latitude, double longitude, int method = 2)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));

            try
            {
                var apiUrl = $"{BaseUrl}/timings?latitude={latitude}&longitude={longitude}&method={method}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API call failed: {response.StatusCode} - {response.ReasonPhrase}");
                    return ApiResponse<PrayerTimings>.Fail($"Failed to fetch prayer times. Status Code: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var prayerData = JsonSerializer.Deserialize<DailyPrayerTimeResponse>(content);

                if (prayerData?.Data?.Timings == null)
                {
                    _logger.LogError("API response format is invalid.");
                    return ApiResponse<PrayerTimings>.Fail("Invalid response from the API.");
                }

                return ApiResponse<PrayerTimings>.Success(prayerData.Data.Timings);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Network error while fetching prayer times: {ex.Message}");
                return ApiResponse<PrayerTimings>.Fail("Network error. Please check your connection.");
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("API request timed out.");
                return ApiResponse<PrayerTimings>.Fail("Request timed out. Try again later.");
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error parsing API response: {ex.Message}");
                return ApiResponse<PrayerTimings>.Fail("Error processing server response.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return ApiResponse<PrayerTimings>.Fail("An unexpected error occurred. Please try again.");
            }
        }

    }
}
