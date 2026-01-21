using HospitalAppointment.DTOs;
using HospitalAppointment_core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public IActionResult Create(AppointmentDTO dto)
        { _appointmentService.CreateAppointment(
            dto.PatientId, 
            dto.DoctorId, 
            dto.AppointmentDateTime);
            return Ok("Created Appointment");
        }
    }
}
