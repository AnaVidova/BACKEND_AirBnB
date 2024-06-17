using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Mysqlx;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace AirBNB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class Camping_PlaceController : ControllerBase
    {
        private readonly MyDatabaseContext _dbContext;


        public Camping_PlaceController(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campingplace>>> GetAllPlaces()
        {
            try
            {
                var places = _dbContext.campingplace.Select(cp => new Models.Campingplace
                {
                    place_id = cp.place_id,
                    owner_id = cp.owner_id,
                    location_id = cp.location_id,
                    name = cp.name,
                    check_in = cp.check_in,
                    check_out = cp.check_out,
                    place_description = cp.place_description
                });
                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCampingPlaceById(int id)
        {
            try
            {
                var place = _dbContext.campingplace
                .Where(cp => cp.place_id == id)
                .Select(cp => new Models.Campingplace
                {
                    place_id = cp.place_id,
                    owner_id = cp.owner_id,
                    location_id = cp.location_id,
                    name = cp.name,
                    check_in = cp.check_in,
                    check_out = cp.check_out,
                    place_description = cp.place_description
                });
                if (place == null)
                {
                    return NotFound();
                }

                return Ok(place);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostCampingPlace(Models.Campingplace cp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var place = new Models.Campingplace
            {

                owner_id = cp.owner_id,
                location_id = cp.location_id,
                name = cp.name,
                check_in = cp.check_in,
                check_out = cp.check_out,
                place_description = cp.place_description

            };

            try
            {
                _dbContext.campingplace.Add(place);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework in a real app)
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return CreatedAtAction(nameof(GetCampingPlaceById), new { id = cp.place_id }, place);
        }
        [HttpGet("user/{user_id}")]
        public async Task<IActionResult> GetAllperUser(int user_id)
        {
            var places = _dbContext.campingplace
                .Where(cp => cp.owner_id == user_id)
                .Select(cp => new Models.Campingplace
                {
                    place_id = cp.place_id,
                    name = cp.name,
                    location_id = cp.location_id
                });

            if (places == null)
            {
                return NotFound();
            }

            return Ok(places);

        }
        [HttpGet("location")]
        public async Task<IActionResult> Filter([FromQuery] string country, [FromQuery] string city)
        {
            var location = _dbContext.location
               .Where(l => l.country == country && l.city == city)
               .Select(l => l.location_id)
               .ToList();


            if (location != null)
            {
                var places = _dbContext.campingplace
                    .Where(cp => location.Contains(cp.location_id))
                    .Select(cp => new Models.Campingplace
                    {
                        place_id = cp.place_id,
                        name = cp.name
                    });
                return Ok(places);
            }
            else
            {
                return NotFound("No places found in that location");
            }

        }
        [HttpGet("price/{id}")]
        public async Task<IActionResult> PriceFilter(int id)
        {
            var maxPrices = _dbContext.campingplace
                .Where(cp => cp.place_id == id)
                .Join(
                    _dbContext.camping_spots,
                    cp => cp.place_id,
                    cs => cs.place_id, 
                    (cp, cs) => new { CampingPlace = cp, CampingSpot = cs }
                )
                .GroupBy(joined => joined.CampingPlace.place_id)
                .Select(group => new
                {
                    PlaceId = group.Key,
                    MaxPricePerNight = group.Max(item => item.CampingSpot.price_night)
                })
                .ToList();

            return Ok(maxPrices);
        }

        [HttpGet("availableplaces/{id}/{maxprice}")]
        public async Task<IActionResult> GetAvailablePlaces(int id, int maxprice)
        {
            var availablePlaces = _dbContext.camping_spots
                    .Where(cs => cs.place_id == id)
                    .GroupJoin(
                        _dbContext.availability,
                        cs => cs.spot_id,
                        av => av.spot_id,
                        (cs, av) => new { CampingSpot = cs, Availabilities = av.DefaultIfEmpty() }
                    )
                    .SelectMany(
                        joined => joined.Availabilities.DefaultIfEmpty(),
                        (joined, av) => new { joined.CampingSpot, Availability = av }
                    )
                    .Where(joined => (joined.Availability == null || joined.Availability.date_end < DateOnly.FromDateTime(DateTime.Now))
                                     && (joined.CampingSpot.price_night <= maxprice)
                                     && (joined.Availability.cur_ind == 1 || joined.Availability.cur_ind == null))
                    .Join(
                        _dbContext.campingplace,
                        cs => cs.CampingSpot.place_id,
                        cp => cp.place_id,
                        (cs, cp) => new { cs.CampingSpot, cs.Availability, CampingPlace = cp }
                    )
                    .Select(joined => new
                    {
                        spot_name = joined.CampingSpot.spot_name,
                        date_end = joined.Availability.date_end != null ? (DateOnly?)joined.Availability.date_end : null,
                        place_name = joined.CampingPlace.name,
                        price_night = joined.CampingSpot.price_night
                    })
                    .ToList();

            return Ok(availablePlaces);
        }
    }
}
