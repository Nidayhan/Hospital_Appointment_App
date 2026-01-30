using HospitalAppointment_core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            var result = _departmentService.DeleteDepartment(id);

            if (!result)
            {

                return NotFound("Department not found or already inactive");
            }
            return Ok("Department deleted successfully");

        }
    }
}
