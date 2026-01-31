using HospitalAppointment_core.DTOs;
using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_core.Interfaces;
using HospitalAppointment_Infrastructure.UoW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class PatientController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PatientController(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        // GET /api/patients/me
        [HttpGet("me")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var currentUserId))
                return Forbid();

            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null) return NotFound();

            var dto = new PatientDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                TcKimlikNo = user.TcKimlikNo
            };

            return Ok(dto);
        }

        // PUT /api/patients/me
        [HttpPut("me")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> UpdateProfile([FromBody] PatientDTO dto)
        {
            if (dto == null) return BadRequest("Invalid payload.");

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var currentUserId))
                return Forbid();

            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null) return NotFound();

            // Do not allow changing TcKimlikNo via this endpoint
            user.FirstName = dto.FirstName?.Trim() ?? user.FirstName;
            user.LastName = dto.LastName?.Trim() ?? user.LastName;
            user.PhoneNumber = dto.PhoneNumber?.Trim() ?? user.PhoneNumber;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        // DELETE /api/patients/me  -> soft delete / deactivate account
        [HttpDelete("me")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var currentUserId))
                return Forbid();

            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null) return NotFound();

            // Soft-delete
            user.IsActive = false;
            await _userRepository.UpdateAsync(user);

            // Important: revoke refresh tokens, sign out sessions, audit the action.
            // Consider adding IRefreshTokenRepository here to revoke tokens.
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
