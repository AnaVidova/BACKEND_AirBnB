using Microsoft.AspNetCore.Mvc;
using Models;

namespace AirBNB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class Amenities_campingController : Controller
    {
        private readonly MyDatabaseContext _dbContext;


        public Amenities_campController(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Amenities_camp>>> GetAllAmenities()
        {
            try
            {
                var amenities = _dbContext.amenities_camp.Select(am => new Models.Amenities_camp
                {
                    am_camp_id = am.am_camp_id,
                    am_id = am.am_id,
                    place_id = am.place_id
                });
                return Ok(amenities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("amenity_names")]
        public async Task<ActionResult<IEnumerable<Amenities_camp>>> GetAmenities()
        {
            try
            {
                var amenities = _dbContext.amenities.Select(am => new Models.Amenities
                {
                    am_id = am.am_id,
                    amenity = am.amenity
                });
                return Ok(amenities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAmenitiesById(int id)
        {
            try
            {
                var amen = _dbContext.amenities_camp
                .Where(am => am.am_camp_id == id)
                .Select(am => new Models.Amenities_camp
                {
                    am_camp_id = am.am_camp_id,
                    am_id = am.am_id,
                    place_id = am.place_id
                });
                if (amen == null)
                {
                    return NotFound();
                }

                return Ok(amen);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAmenities(Models.Amenities_camp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var amenitiesCamp = new Models.Amenities_camp
            {
                am_id = model.am_id,
                place_id = model.place_id
            };

            _dbContext.amenities_camp.Add(amenitiesCamp);
            _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAmenitiesById), new { id = amenitiesCamp.am_camp_id }, amenitiesCamp);
        }
    }
}
