using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AirBNB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ReviewsController : ControllerBase
    {
        private readonly MyDatabaseContext _dbContext;


        public ReviewsController(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reviews>>> GetAllReviews()
        {
            try
            {
                var reviews =  _dbContext.reviews.Select(r => new Models.Reviews
                {
                    review_id = r.review_id,
                    user_id = r.user_id,
                    place_id = r.place_id,
                    description = r.description,
                    date_posted = r.date_posted,
                    stars = r.stars
                    
                });
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetReviewById(int id)
        {
            try
            {
                var review = _dbContext.reviews
                .Where(r => r.review_id == id)
                .Select(r => new Models.Reviews
                {
                    review_id = r.review_id,
                    user_id = r.user_id,
                    place_id = r.place_id,
                    description = r.description,
                    date_posted = r.date_posted,
                    stars = r.stars
                });
                if (review == null)
                {
                    return NotFound();
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostReview(Models.Reviews r)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviews = new Models.Reviews
            {
                user_id = r.user_id,
                place_id = r.place_id,
                description = r.description,
                date_posted = r.date_posted,
                stars = r.stars
            };

            try
            {
                _dbContext.reviews.Add(reviews);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework in a real app)
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return CreatedAtAction(nameof(GetReviewById), new { id = reviews.review_id }, reviews);
        }
        [HttpGet("place/{id}")]
        public async Task<ActionResult> GetReviewsByPlace(int id)
        {
            try
            {
                var reservation = _dbContext.reviews
                .Where(r => r.place_id == id)
                .Select(r => new Models.Reviews
                {
                    review_id = r.review_id,
                    user_id = r.user_id,
                    place_id = r.place_id,
                    description = r.description,
                    date_posted = r.date_posted,
                    stars = r.stars
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
