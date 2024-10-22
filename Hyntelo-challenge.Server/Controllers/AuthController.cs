using Hyntelo_challenge.Server.DTO;
using Hyntelo_challenge.Server.Models;
using Hyntelo_challenge.Server.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hyntelo_challenge.Server.Controllers
{
    /// <summary>
    /// Controller responsible for handling user authentication operations.
    /// Implements JWT (JSON Web Token) based authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the AuthController.
        /// </summary>
        /// <param name="unitOfWork">Repository pattern implementation for database operations.</param>
        /// <param name="configuration">Application configuration containing JWT settings.</param>
        public AuthController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Authenticates a user and issues a JWT token.
        /// </summary>
        /// <param name="login">The login credentials containing username and password.</param>
        /// <returns>
        /// 200 OK with JWT token if authentication is successful.
        /// 401 Unauthorized if credentials are invalid.
        /// 400 Bad Request if configuration is invalid.
        /// </returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/login
        ///     {
        ///         "username": "user@example.com",
        ///         "password": "userPassword"
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///         "token": "eyJhbGciOiJIUzI1NiIs..."
        ///     }
        /// </remarks>
        /// <response code="200">Returns the JWT token</response>
        /// <response code="401">If the credentials are invalid</response>
        /// <response code="400">If the server configuration is invalid</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            // Retrieve the user from the database based on the username
            var user = await _unitOfWork.UserRepository.GetByUserName(login.Username ?? "");

            //TODO: Security improvement needed - implement salted hash password storage in database
            if (user == null || (login.Password != user.Password))
            {
                // Return Unauthorized if the user is not found or the password does not match
                return Unauthorized("Invalid credentials");
            }

            // Create claims for the JWT token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("username", login.Username)
            };

            // Get the secret key from configuration
            var secret = _configuration["Jwt:SecretKey"];
            if (secret == null)
            {
                return ValidationProblem("Invalid configuration");
            }

            // Create security key and signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Generate JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            // Return the token
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}