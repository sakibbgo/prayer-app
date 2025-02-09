using Microsoft.AspNetCore.Mvc;
using PrayerTimes.API.Models;
using PrayerTimes.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrayerTimes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrayerTimeController : ControllerBase
    {
        private readonly PrayerTimeService _prayerTimeService;

        public PrayerTimeController(PrayerTimeService prayerTimeService)
        {
            _prayerTimeService = prayerTimeService;
        }

        // Improved: Get daily prayer times
        [HttpGet("daily/{city}/{country}")]
        public async Task<IActionResult> GetDailyPrayerTimes(string city, string country)
        {
            var prayerTimes = await _prayerTimeService.GetDailyPrayerTimesAsync(city, country);

            if (prayerTimes == null)
                return NotFound(new { message = "Could not fetch prayer times. Check city/country name or try again later." });

            return Ok(prayerTimes);
        }

        // Improved: Get monthly prayer times
        [HttpGet("monthly/{city}/{country}/{month}/{year}")]
        public async Task<IActionResult> GetMonthlyPrayerTimes(string city, string country, int month, int year)
        {
            var prayerTimes = await _prayerTimeService.GetMonthlyPrayerTimesAsync(city, country, month, year);

            if (prayerTimes == null || prayerTimes.Count == 0)
                return NotFound(new { message = "Could not fetch monthly prayer times. Try again later." });

            return Ok(prayerTimes);
        }

        // Improved: Get prayer times by coordinates
        [HttpGet("coordinates")]
        public async Task<IActionResult> GetPrayerTimesByCoordinates([FromQuery] double lat, [FromQuery] double lon, [FromQuery] int method = 2)
        {
            var prayerTimes = await _prayerTimeService.GetPrayerTimesByCoordinatesAsync(lat, lon, method);

            if (prayerTimes == null)
                return NotFound(new { message = "Could not fetch prayer times for given coordinates." });

            return Ok(prayerTimes);
        }
    }
}
