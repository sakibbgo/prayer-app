using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PrayerTimes.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // Fetches daily prayer times for a city with error handling
        public async Task<PrayerTimings?> GetDailyPrayerTimesAsync(string city, string country)
        {
            try
            {
                var apiUrl = $"{BaseUrl}/timingsByCity?city={city}&country={country}";
                return await FetchPrayerTimes(apiUrl);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error fetching daily prayer times: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON deserialization error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return null;
            }
        }

        // Fetches monthly prayer times with error handling
        public async Task<List<PrayerTimings>?> GetMonthlyPrayerTimesAsync(string city, string country, int month, int year)
        {
            try
            {
                var apiUrl = $"{BaseUrl}/calendarByCity?city={city}&country={country}&month={month}&year={year}";
                return await FetchMonthlyPrayerTimes(apiUrl);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error fetching monthly prayer times: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON deserialization error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return null;
            }
        }

        // Fetches prayer times by GPS coordinates with error handling
        public async Task<PrayerTimings?> GetPrayerTimesByCoordinatesAsync(double latitude, double longitude, int method = 2)
        {
            try
            {
                var apiUrl = $"{BaseUrl}/timings?latitude={latitude}&longitude={longitude}&method={method}";
                return await FetchPrayerTimes(apiUrl);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error fetching prayer times by coordinates: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON deserialization error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return null;
            }
        }

        // Generic method to fetch daily prayer times
        private async Task<PrayerTimings?> FetchPrayerTimes(string apiUrl)
        {
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API call failed: {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var prayerData = JsonSerializer.Deserialize<DailyPrayerTimeResponse>(content);
            return prayerData?.Data?.Timings;
        }

        // Generic method to fetch monthly prayer times
        private async Task<List<PrayerTimings>?> FetchMonthlyPrayerTimes(string apiUrl)
        {
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API call failed: {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var prayerData = JsonSerializer.Deserialize<MonthlyPrayerTimesResponse>(content);
            return prayerData?.Data?.Select(d => d.Timings).ToList();
        }
    }
}
