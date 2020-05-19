using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingAppAPI.Data;
using DatingAppAPI.Dtos;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository authRepository,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto userRegistration)
        {
            userRegistration.Username = userRegistration.Username.ToLower();

            if (await _authRepository.UserExists(userRegistration.Username))
                return BadRequest($"Username {userRegistration.Username} already exists.");

            var userToCreate = new User
            {
                Username = userRegistration.Username
            };

            var createdUser = await _authRepository.Register(userToCreate, userRegistration.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            var storedUser = await _authRepository.Login(userLogin.Username.ToLower(), userLogin.Password);

            if (storedUser == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, storedUser.Id.ToString()),
                new Claim(ClaimTypes.Name, storedUser.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:TokenSecret"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = signingCredentials
            });

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}
