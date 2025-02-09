using System.Text.Json.Serialization;

namespace PrayerTimes.API.Models
{
    public class PrayerTimings // Renamed from PrayerTimes to PrayerTimings
    {
        [JsonPropertyName("Fajr")]
        public string Fajr { get; set; } = string.Empty;

        [JsonPropertyName("Dhuhr")]
        public string Dhuhr { get; set; } = string.Empty;

        [JsonPropertyName("Asr")]
        public string Asr { get; set; } = string.Empty;

        [JsonPropertyName("Maghrib")]
        public string Maghrib { get; set; } = string.Empty;

        [JsonPropertyName("Isha")]
        public string Isha { get; set; } = string.Empty;
    }

    // Daily Prayer Times Response
    public class DailyPrayerTimeResponse
    {
        [JsonPropertyName("data")]
        public DailyPrayerData Data { get; set; } = new DailyPrayerData();
    }

    public class DailyPrayerData
    {
        [JsonPropertyName("timings")]
        public PrayerTimings Timings { get; set; } = new PrayerTimings();
    }

    // Monthly Prayer Times Response
    public class MonthlyPrayerTimesResponse
    {
        [JsonPropertyName("data")]
        public List<MonthlyPrayerEntry> Data { get; set; } = new List<MonthlyPrayerEntry>();
    }

    public class MonthlyPrayerEntry
    {
        [JsonPropertyName("date")]
        public PrayerDateInfo Date { get; set; } = new PrayerDateInfo();

        [JsonPropertyName("timings")]
        public PrayerTimings Timings { get; set; } = new PrayerTimings();
    }

    public class PrayerDateInfo
    {
        [JsonPropertyName("readable")]
        public string Readable { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
    }
}
