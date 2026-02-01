using HospitalAppointment_core.DTOs;
using HospitalAppointment_core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        // Patients (or Admin) can create appointments.
        [HttpPost]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDTO dto)
        {
            // model binding check (gives clearer 400s)
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create appointment: invalid modelstate: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                // read claims
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userIdClaim = User.FindFirst("userId")?.Value;
                _logger.LogDebug("Create appointment called. Claims: role={Role}, userIdClaim={UserIdClaim}. DTO.PatientId={PatientId}",
                    role, userIdClaim, dto.PatientId);

                if (!int.TryParse(userIdClaim, out var currentUserId))
                {
                    _logger.LogWarning("Create appointment: could not parse userId claim: {UserIdClaim}", userIdClaim);
                    return Forbid();
                }

                // SECURITY: For non-admin callers, ignore any patientId supplied by client and use the caller's id.
                // This prevents token/client mismatches and enforces "patients can only create for themselves".
                if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    if (dto.PatientId != currentUserId)
                        _logger.LogInformation("Overriding DTO.PatientId {DtoPatientId} with caller id {CurrentUserId}", dto.PatientId, currentUserId);

                    dto.PatientId = currentUserId;
                }

                await _appointmentService.CreateAppointment(
                    dto.PatientId,
                    dto.DoctorId,
                    dto.AppointmentDateTime
                );

                return Ok("Created Appointment");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment for patientId={PatientId}", dto?.PatientId);
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
