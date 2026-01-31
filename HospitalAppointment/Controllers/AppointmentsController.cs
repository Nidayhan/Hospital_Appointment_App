using HospitalAppointment_core.DTOs;
using HospitalAppointment_core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // All endpoints require authentication; method-level roles enforced below.
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // Patients (or Admin) can create appointments.
        [HttpPost]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> Create([FromBody] AppointmentResponseDTO dto)
        {
            try
            {
                // Claim-based check: patient can only create for self (unless Admin)
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userIdClaim = User.FindFirst("userId")?.Value;

                if (!int.TryParse(userIdClaim, out var currentUserId))
                    return Forbid();

                if (role != "Admin" && dto.PatientId != currentUserId)
                    return Forbid("Patients can only create appointments for themselves.");

                await _appointmentService.CreateAppointment(
                    dto.PatientId,
                    dto.DoctorId,
                    dto.AppointmentDateTime
                );

                return Ok("Created Appointment");
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        // Patient can cancel their own appointment. We pass currentUserId to business layer.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!int.TryParse(userIdClaim, out var currentUserId))
                    return Forbid();

                await _appointmentService.CancelAppointment(id, currentUserId);
                return Ok("Appointment cancelled");
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        // Patients can view their own appointments
        [HttpGet("me")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyAppointments()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!int.TryParse(userIdClaim, out var currentUserId))
                    return Forbid();

                var appointments = await _appointmentService.GetAppointmentsByPatientAsync(currentUserId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
