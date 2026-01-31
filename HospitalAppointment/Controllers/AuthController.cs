using HospitalAppointment_core.DTOs;
using HospitalAppointment_core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Created(string.Empty, response);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (InvalidOperationException ioex)
            {
                return Conflict(ioex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException)
            {
                return Unauthorized("Invalid credentials.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        public class LogoutRequest
        {
            public string RefreshToken { get; set; } = string.Empty;
        }

        // Log out: revoke refresh token. Requires authentication to ensure the caller is the token owner.
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!int.TryParse(userIdClaim, out var currentUserId))
                    return Forbid();

                await _authService.LogoutAsync(request.RefreshToken, currentUserId);
                return NoContent();
            }
            catch (InvalidOperationException ioex)
            {
                return BadRequest(ioex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
