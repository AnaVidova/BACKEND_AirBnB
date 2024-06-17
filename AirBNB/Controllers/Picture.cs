using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AirBNB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PictureController : ControllerBase
    {
        private readonly MyDatabaseContext _dbContext;


        public PictureController(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] int placeId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            var picture = new Picture
            {
                file_name = file.FileName,
                content_type = file.ContentType,
                place_id = placeId
            };

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                picture.data = ms.ToArray();
            }

            _dbContext.picture.Add(picture);
            await _dbContext.SaveChangesAsync();

            return Ok(new { picture.pic_id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var picture = await _dbContext.picture.FindAsync(id);

            if (picture == null)
                return NotFound();

            return File(picture.data, picture.content_type, picture.file_name);
        }

        [HttpGet("place/{place_id}")]
        public async Task<IActionResult> GetPlacePic(int place_id)
        {
            var pictures = await _dbContext.picture
                .Where(pic => pic.place_id == place_id)
                .Select(pic => new
                {
                    Data = pic.data,
                    ContentType = pic.content_type
                })
                .ToListAsync();

            if (pictures == null || pictures.Count == 0)
                return NotFound();


            return Ok(pictures);
        }

    }
}
