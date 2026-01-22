using HospitalAppointment_core.DTOs;
using HospitalAppointment_core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost("register")]
        public IActionResult Register(PatientDTO dto)
        {
            var result = _patientService.CreatePatient(dto);

            if (!result)
                return BadRequest("Bu TC kimlik numarası ile kayıt zaten mevcut");
            return Ok("hasta başarıyla kaydedildi.");
        }

    }
}
