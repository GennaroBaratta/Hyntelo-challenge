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

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly IConfiguration _configuration;

        public AuthController(IUnitOfWork unitOfWork,IConfiguration configuration)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            // Validate the credentials (this is a basic example; in production, use proper identity/auth services)
            if (login.Username == "admin" && login.Password == "password")
            {
                var user = await _unitOfWork.UserRepository.GetByUserName(login.Username);

                // Create JWT token
                var claims = new[]
                { 
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  
                    new Claim("username", login.Username)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: _configuration["Jwt:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized("Invalid credentials");
        }
    }
}
