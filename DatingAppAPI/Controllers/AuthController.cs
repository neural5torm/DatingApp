using System.Threading.Tasks;
using DatingAppAPI.Data;
using DatingAppAPI.Dtos;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
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
    }
}
