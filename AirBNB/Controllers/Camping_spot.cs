using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AirBNB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class Camping_spot : ControllerBase
    {
        private readonly MyDatabaseContext _dbContext;


        public Camping_spot(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Camping_spots>>> GetAllSpots()
        {
            try
            {
                var spots = _dbContext.camping_spots.Select(csp => new Models.Camping_spots
                {
                    spot_id = csp.spot_id,
                    spot_name = csp.spot_name,
                    price_night = csp.price_night,
                    type_name = csp.type_name,
                    place_id = csp.place_id
                });
                return Ok(spots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCampingSpotById(int id)
        {
            try
            {
                var spot = _dbContext.camping_spots
                .Where(csp => csp.spot_id == id)
                .Select(csp => new Models.Camping_spots
                {

                    spot_id = csp.spot_id,
                    spot_name = csp.spot_name,
                    price_night = csp.price_night,
                    type_name = csp.type_name,
                    place_id = csp.place_id

                });
                if (spot == null)
                {
                    return NotFound();
                }

                return Ok(spot);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostCampingSpot(Models.Camping_spots csp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var spot = new Models.Camping_spots
            {
                spot_name = csp.spot_name,
                price_night = csp.price_night,
                type_name = csp.type_name,
                place_id = csp.place_id

            };

            try
            {
                _dbContext.camping_spots.Add(spot);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework in a real app)
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return CreatedAtAction(nameof(GetCampingSpotById), new { id = csp.spot_id }, spot);
        }

        [HttpGet("campingplace/{id}")]
        public async Task<ActionResult> GetCampingSpotByCampingPlace(int id)
        {
            try
            {
                var spot = _dbContext.camping_spots
                .Where(csp => csp.place_id == id)
                .Select(csp => new Models.Camping_spots
                {

                    spot_id = csp.spot_id,
                    spot_name = csp.spot_name,
                    price_night = csp.price_night,
                    type_name = csp.type_name,
                    place_id = csp.place_id

                });
                if (spot == null)
                {
                    return NotFound();
                }

                return Ok(spot);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
    }
}
