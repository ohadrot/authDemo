using authDemo.Contexts;
using authDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace authDemo.Controllers
{
    [ApiController]
    [Route("")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly UserContext _context;

        public WeatherForecastController(UserContext context, ILogger<WeatherForecastController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            // Add the user to the context
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Return the created user
            return CreatedAtAction(nameof(CreateUser), new { id = user.Id }, user);
        }

        [HttpGet("home")]
        public async Task<ActionResult<string>> Home()
        {
            return Ok("welcome");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            if (loginUser == null || string.IsNullOrEmpty(loginUser.Username) || string.IsNullOrEmpty(loginUser.Password))
            {
                return BadRequest("Invalid username or password.");
            }

            // Check if the username and password exist in the database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

            if (user == null)
            {
                return Unauthorized("Username or password is incorrect.");
            }

            // Generate a token (for simplicity, a constant string is used here)
            var token = "aaa324dsfvdfrwferw"; // In practice, generate a unique token per user/session

            

            // Set the token as a cookie with the "Authorization" attribute
            Response.Cookies.Append("Authorization", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure this is only sent over HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1) // Set appropriate expiration
            });

            // Return the token or a success message
            return Ok(new { Token = token });
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            // Delete the "Authorization" cookie
            Response.Cookies.Delete("Authorization");

            // Return a message indicating that the user has been logged out
            return Ok("Authorization cookie has been deleted.");
        }

    }
}

    