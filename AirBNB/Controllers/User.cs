using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BCrypt.Net;


namespace AirBNB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User : ControllerBase
    {
        private readonly MyDatabaseContext _dbContext;

        public User(MyDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Users>>> GetAllUsers()
        {
            try
            {
                var users = _dbContext.users
                    .Select(u => new Users
                    {
                        user_id = u.user_id,
                        first_name = u.first_name,
                        last_name = u.last_name,
                        pic_data = u.pic_data,
                        email = u.email,
                        telephone = u.telephone,
                        password = u.password,
                        is_owner = u.is_owner,
                        content_type = u.content_type
                    });
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }


        //'$2a$10$8xEW4R5WSR1C2OSGMUPVD.aJG9.xWT7tMNJOxHFJaYhy78tS2gabO'
        [HttpGet("login/{email}/{password}")]
        public async Task<ActionResult> Login(string email, string password)
        {
            try
            {
                
                var user = await _dbContext.users.FirstOrDefaultAsync(u => u.email == email);
                if (user == null)
                {
                    return NotFound(" Invalid email.");
                }
                Console.WriteLine($"{email} {password} {user.password} {BCrypt.Net.BCrypt.Verify(password, user.password)}");

                if (!BCrypt.Net.BCrypt.Verify(password, user.password))
                {
                    // Passwords don't match
                    
                    return NotFound($"Invalid password. {password} {user.password}");
                }
                var userDetails = new Models.Users
                {
                    user_id = user.user_id,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    pic_data = user.pic_data,
                    email = user.email,
                    telephone = user.telephone,
                    password = user.password,
                    is_owner = user.is_owner,
                    content_type = user.content_type
                };
            
                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(int id)
        {
            try
            {
                var user = _dbContext.users
                .Where(u => u.user_id == id)
                .Select(u => new Models.Users
                {
                    user_id = u.user_id,
                    first_name = u.first_name,
                    last_name = u.last_name,
                    pic_data = u.pic_data,
                    email = u.email,
                    telephone = u.telephone,
                    password = u.password,
                    is_owner = u.is_owner,
                    content_type = u.content_type
                });
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostUsers(IFormFile file, [FromForm] Models.UsersPicture model)
        {
              if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usersadd = new Users
            {
                first_name = model.first_name,
                last_name = model.last_name,
                email = model.email,
                telephone = model.telephone,
                password = HashPassword(model.password),
                is_owner = model.is_owner,
            };

            if (file != null && file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    usersadd.pic_data = ms.ToArray();
                    usersadd.content_type = file.ContentType;
                }
            }

            try
            {
                _dbContext.users.Add(usersadd);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework in a real app)
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return CreatedAtAction(nameof(GetUserById), new { id = usersadd.user_id }, usersadd);
        }

        [HttpGet("users/{user_id}/picture")]
        public async Task<IActionResult> GetPicture(int user_id)
        {
            // Retrieve the user by ID
            var user = await _dbContext.users.FindAsync(user_id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Check if the user has a picture
            if (user.pic_data == null || user.pic_data.Length == 0)
            {
                return NotFound("Picture not found for this user");
            }



            // Return the picture as a file response
            return File(user.pic_data, user.content_type);
        }

        [HttpPut("{email}/{newPassword}")]
        public async Task<IActionResult> ChangePassword(string email, string newPassword)
        {
            // Find the user by email
            //var user = _dbContext.users.Where(user => user.email == email).ExecuteUpdate(setters => setters.SetProperty(users => users.password, HashPassword(newPassword)));
            var user =  await _dbContext.users.FirstOrDefaultAsync(u => u.email == email);
            if (user == null)
            {
                return NotFound(); // Return a 404 if user not found
            }

            string hashed = HashPassword(newPassword);
            user.password = hashed;
            // Save changes to the database
            _dbContext.users.Update(user);
            _dbContext.SaveChangesAsync();

            return Ok($"{user.password} {hashed}"); // Return a 200 OK response
        }

        [HttpPut("update/{email}")]
        public async Task<IActionResult> ChangeUserInfo(string email, [FromBody] Models.UpdatedUser updated)
        {

            var user = await _dbContext.users.FirstOrDefaultAsync(u => u.email == email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.first_name = updated.first_name;
            user.last_name = updated.last_name;
            user.email = updated.email;
            user.telephone = updated.telephone;
            user.is_owner = updated.is_owner;

            try
            {
                _dbContext.users.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return NoContent();

        }
        [HttpPut("update/picture/{user_id}")]
        public async Task<IActionResult> ChangeUserInfo(int user_id, IFormFile file)
        {

            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            var user = await _dbContext.users.FirstOrDefaultAsync(u => u.user_id == user_id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (file != null && file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    user.pic_data = ms.ToArray();
                    user.content_type = file.ContentType;
                }
            }

            try
            {
                // Handle DBNull for pic_data
                if (user.pic_data is DBNull)
                {
                    // If pic_data is DBNull, populate it with an empty byte array
                    user.pic_data = new byte[0];
                }   

                // Handle DBNull for content_type
                if (user.content_type is DBNull)
                {
                    // If content_type is DBNull, populate it with an empty string
                    user.content_type = string.Empty;
                }

                _dbContext.users.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return NoContent();

        }
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
