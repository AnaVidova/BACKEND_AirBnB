using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AirBNB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypesController : ControllerBase
    {
        private readonly MyDatabaseContext _dbContext;

        public TypesController(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Types>>> GetAllTypes()
        {
            try
            {
                var types = _dbContext.type.Select(t => new Models.Types
                {
                    type_id = t.type_id,
                    name_type = t.name_type
                });
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
