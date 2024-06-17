using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;

namespace AirBNB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class LocationController : ControllerBase
    {
        private readonly MyDatabaseContext _dbContext;


        public LocationController(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetAllLocations()
        {
            try
            {
                var locations = _dbContext.location.Select(l => new Models.Location
                {
                    location_id = l.location_id,
                    country = l.country,
                    city = l.city,
                    street = l.street,
                    zipcode = l.zipcode,
                    full_address = l.full_address

                });
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetLocationById(int id)
        {
            try
            {
                var locationvar = _dbContext.location
                .Where(l => l.location_id == id)
                .Select(l => new Models.Location
                {

                    location_id = l.location_id,
                    country = l.country,
                    city = l.city,
                    street = l.street,
                    zipcode = l.zipcode,
                    full_address = l.full_address

                });
                if (locationvar == null)
                {
                    return NotFound();
                }

                return Ok(locationvar);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostLocation(Models.Location l)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locationvar = new Models.Location
            {
                country = l.country,
                city = l.city,
                street = l.street,
                zipcode = l.zipcode,
                full_address = l.full_address

            };

            try
            {
                _dbContext.location.Add(locationvar);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework in a real app)
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return CreatedAtAction(nameof(GetLocationById), new { id = l.location_id }, locationvar);
        }

    }
}
