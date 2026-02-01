using HospitalAppointment_core.DTOs;
using HospitalAppointment_core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;

        public DoctorController(IDoctorService doctorService, IAppointmentService appointmentService)
        {
            _doctorService = doctorService;
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorCreateDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!int.TryParse(userIdClaim, out var adminUserId))
                    return Forbid();

                var created = await _doctorService.CreateDoctorAsync(dto, adminUserId);
                return CreatedAtAction(nameof(GetDoctorById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ioex)
            {
                return BadRequest(ioex.Message);
            }
            catch (System.Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();

            var response = new
            {
                doctor.Id,
                doctor.UserId,
                doctor.Name,
                doctor.Specialty,
                doctor.DepartmentId,
                DepartmentName = doctor.Department?.Name,
                doctor.IsActive
            };

            return Ok(response);
        }

        // New endpoint: available slots for doctor (date query param is optional; server expects UTC date)
        [HttpGet("{id}/available-slots")]
        [AllowAnonymous] // adjust authorization if you want only authenticated users
        public async Task<IActionResult> GetAvailableSlots(int id, [FromQuery] DateTime? date)
        {
            var queryDateUtc = date.HasValue
                ? (date.Value.Kind == DateTimeKind.Utc ? date.Value.Date : date.Value.ToUniversalTime().Date)
                : DateTime.UtcNow.Date;

            var slots = await _appointmentService.GetAvailableSlotsAsync(id, queryDateUtc);
            return Ok(slots);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorUpdateDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!int.TryParse(userIdClaim, out var adminUserId))
                    return Forbid();

                var updated = await _doctorService.UpdateDoctorAsync(id, dto, adminUserId);
                return Ok(new
                {
                    updated.Id,
                    updated.UserId,
                    updated.Name,
                    updated.Specialty,
                    updated.DepartmentId,
                    DepartmentName = updated.Department?.Name,
                    updated.IsActive
                });
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (InvalidOperationException ioex)
            {
                return NotFound(ioex.Message);
            }
            catch (System.Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!int.TryParse(userIdClaim, out var adminUserId))
                    return Forbid();

                var result = await _doctorService.DeleteDoctorAsync(id, adminUserId);
                if (!result) return NotFound("Doctor not found or already inactive");
                return Ok("Doctor deleted successfully");
            }
            catch (System.Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
