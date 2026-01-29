using HospitalAppointment_core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;
        public AuthController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public IActionResult Login(string role)
        {
            // fake user
            var userId = role switch
            {
                "Admin" => 1,
                "Doctor" => 2,
                "Patient" => 3,
                _ => 0
            };

            var token = _jwtTokenService.GenerateToken(userId, role);
            return Ok(token);
        }
    }
}
