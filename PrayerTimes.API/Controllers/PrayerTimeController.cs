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

        // Improved: Get prayer times by coordinates
        [HttpGet("daily-by-coordinates")]
        public async Task<IActionResult> GetDailyPrayerTimesByCoordinates(double latitude, double longitude)
        {
            var result = await _prayerTimeService.GetDailyPrayerTimesByCoordinatesAsync(latitude, longitude);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.ErrorMessage });

            return Ok(result.Data);
        }

    }
}
