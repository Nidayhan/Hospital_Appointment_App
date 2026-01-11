using HospitalAppointment_core.Services;
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
        public IActionResult CreateAppointment(int patientId, int doctorId, DateTime dateTime)
        { _appointmentService.CreateAppointment(patientId, doctorId, dateTime);
            return Ok();
        }
    }
}
