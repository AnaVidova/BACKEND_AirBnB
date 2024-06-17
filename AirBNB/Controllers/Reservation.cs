using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AirBNB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ReservationController : ControllerBase
    {
        private readonly MyDatabaseContext _dbContext;


        public ReservationController(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservations>>> GetAllReservations()
        {
            try
            {
                var reserve = _dbContext.reservations.Select(res => new Models.Reservations
                {
                    id = res.id,
                    user_id = res.user_id,
                    spot_id = res.spot_id,
                    price_total = res.price_total,
                    place_id = res.place_id,
                    date_start = res.date_start,
                    date_end = res.date_end


                });
                return Ok(reserve);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{idreserve}")]
        public async Task<ActionResult> GetReservationById(int idreserve)
        {
            try
            {
                var reservation = _dbContext.reservations
                .Where(res => res.id == idreserve)
                .Select(res => new Models.Reservations
                {

                    id = res.id,
                    user_id = res.user_id,
                    spot_id = res.spot_id,
                    price_total = res.price_total,
                    place_id = res.place_id,
                    date_start = res.date_start,
                    date_end = res.date_end

                });
                if (reservation == null)
                {
                    return NotFound();
                }

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostReservation(Models.Reservations res)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var startDateStr = res.date_start.ToString("yyyy-MM-dd");
            var endDateStr = res.date_end.ToString("yyyy-MM-dd");

            System.DateOnly startDate;
            System.DateOnly endDate;

            if (System.DateOnly.TryParseExact(startDateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startDate))
            {
                res.date_start = startDate;
            }

            if (System.DateOnly.TryParseExact(endDateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out endDate))
            {
                res.date_end = endDate;
                Console.WriteLine(endDate);
            }



            var reservation = new Models.Reservations
            {
                user_id = res.user_id,
                spot_id = res.spot_id,
                price_total = res.price_total,
                place_id = res.place_id,
                date_start = res.date_start,
                date_end = res.date_end

            };

            try
            {
                _dbContext.reservations.Add(reservation);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework in a real app)
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetReservationById), new { id = res.id }, reservation);
        }
        [HttpGet("userres/{user_id}")]
        public async Task<ActionResult> GetReservationsByUser(int user_id)
        {
            try
            {
                var reservation = _dbContext.reservations
                .Where(res => res.user_id == user_id)
                .Select(res => new Models.Reservations
                {

                    id = res.id,
                    user_id = res.user_id,
                    spot_id = res.spot_id,
                    price_total = res.price_total,
                    place_id = res.place_id,
                    date_start = res.date_start,
                    date_end = res.date_end

                });
                if (reservation == null)
                {
                    return NotFound();
                }

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpGet("spot/{spot_id}")]
        public async Task<ActionResult> GetResBySpot(int spot_id)
        {
            try
            {
                var reservation = _dbContext.reservations
                .Where(res => res.spot_id == spot_id)
                .Select(res => new Models.Reservations
                {

                    id = res.id,
                    user_id = res.user_id,
                    spot_id = res.spot_id,
                    price_total = res.price_total,
                    place_id = res.place_id,
                    date_start = res.date_start,
                    date_end = res.date_end

                });
                if (reservation == null)
                {
                    return NotFound();
                }

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }

        }
        [HttpGet("userid/{user_id}")]
        public async Task<ActionResult> GetResByUser(int user_id)
        {
            try
            {
                var userId = 8;

                var reservations = _dbContext.reservations
                    .Where(res => res.user_id == user_id)
                    .Join(
                        _dbContext.camping_spots,
                        res => res.spot_id,
                        cs => cs.spot_id,
                        (res, cs) => new { Reservation = res, CampingSpot = cs }
                    )
                    .Join(
                        _dbContext.campingplace,
                        res_cs => res_cs.Reservation.place_id,
                        cp => cp.place_id,
                        (res_cs, cp) => new { res_cs.Reservation, res_cs.CampingSpot, CampingPlace = cp }
                    )
                    .Select(joined => new
                    {
                        reservation_id = joined.Reservation.id,
                        date_start = joined.Reservation.date_start,
                        date_end = joined.Reservation.date_end,
                        place_name = joined.CampingPlace.name,
                        spot_name = joined.CampingSpot.spot_name,
                        price_total = joined.Reservation.price_total,
                        place_id = joined.CampingPlace.place_id
                    })
                    .ToList();
                return Ok( reservations );

            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpGet("place/{place_id}")]
        public async Task<ActionResult> GetResByPlace(int place_id)
        {
            try
            {
                var reservation = _dbContext.reservations
                .Where(res => res.place_id == place_id)
                .Select(res => new Models.Reservations
                {

                    id = res.id,
                    user_id = res.user_id,
                    spot_id = res.spot_id,
                    price_total = res.price_total,
                    place_id = res.place_id,
                    date_start = res.date_start,
                    date_end = res.date_end

                });
                if (reservation == null)
                {
                    return NotFound();
                }

                return Ok(reservation);
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
