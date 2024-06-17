using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AirBNB.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class AvailabilityController : ControllerBase
    {
        private readonly MyDatabaseContext _dbContext;


        public AvailabilityController(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<availability>>> GetAllAvailability()
        {
            try
            {
                var availabilities = _dbContext.availability.Select(av => new Models.availability
                {
                    avb_id = av.avb_id,
                    spot_id = av.spot_id,
                    date_start = av.date_start,
                    date_end = av.date_end,
                    cur_ind = av.cur_ind
                });
                return Ok(availabilities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAvailabilityById(int id)
        {
            try
            {
                var avb = _dbContext.availability
                .Where(av => av.avb_id == id)
                .Select(av => new Models.availability
                {
                    avb_id = av.avb_id,
                    spot_id = av.spot_id,
                    date_start = av.date_start,
                    date_end = av.date_end,
                    cur_ind = av.cur_ind
                });
                if (avb == null)
                {
                    return NotFound();
                }

                return Ok(avb);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostAvailability(Models.availability av)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var startDateStr = av.date_start.ToString("yyyy-MM-dd");
            var endDateStr = av.date_end.ToString("yyyy-MM-dd");

            System.DateOnly startDate;
            System.DateOnly endDate;

            if (System.DateOnly.TryParseExact(startDateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startDate))
            {
                av.date_start = startDate;
            }

            if (System.DateOnly.TryParseExact(endDateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out endDate))
            {
                av.date_end = endDate;
                Console.WriteLine(endDate);
            }

            var avb = new Models.availability
            {
                spot_id = av.spot_id,
                date_start = av.date_start,
                date_end = av.date_end,
                cur_ind =  av.cur_ind

            };

            try
            {
                _dbContext.availability.Add(avb);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework in a real app)
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return CreatedAtAction(nameof(GetAvailabilityById), new { id = av.avb_id }, avb);
        }
        [HttpGet("spots/{id}")]
        public async Task<ActionResult> GetAvailabilityBySpotsId(int id)
        {
            try
            {
                var avb = _dbContext.availability
                .Where(av => av.spot_id == id)
                .Select(av => new Models.availability
                {
                    avb_id = av.avb_id,
                    spot_id = av.spot_id,
                    date_start = av.date_start,
                    date_end = av.date_end,
                    cur_ind = av.cur_ind
                });
                if (avb == null)
                {
                    return NotFound();
                }

                return Ok(avb);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpPut("updateavailability/{spot_id}")]
        public async Task<IActionResult> ChangeUserInfo(int spot_id)
        {

            var spots = await _dbContext.availability.Where(av => (av.spot_id == spot_id) && (av.date_end < DateOnly.FromDateTime(DateTime.Now))).ToListAsync();

            if (spots == null)
            {
                return NotFound("Spot not found.");
            }

            try
            {
                foreach (var spot in spots)
                {
                    spot.cur_ind = 0;
                }
                _dbContext.availability.UpdateRange(spots);
                _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return NoContent();

        }
    }
}
