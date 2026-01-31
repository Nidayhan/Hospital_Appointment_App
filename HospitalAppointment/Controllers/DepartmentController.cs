using HospitalAppointment_core.Interfaces;
using HospitalAppointment_core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HospitalAppointment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDTO dto)
        {
            try
            {
                var created = await _departmentService.CreateDepartment(dto);
                return CreatedAtAction(nameof(GetDepartmentById), new { id = created.Id }, created);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetDepartmentById(int id)
        {
            var department = _departmentService.GetDepartmentById(id);
            if (department == null) return NotFound();
            return Ok(department);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentDTO dto)
        {
            try
            {
                var updated = await _departmentService.UpdateDepartmentAsync(id, dto);
                return Ok(updated);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (InvalidOperationException ioex)
            {
                return NotFound(ioex.Message);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _departmentService.DeleteDepartment(id);

            if (!result)
            {
                return NotFound("Department not found or already inactive");
            }
            return Ok("Department deleted successfully");
        }
    }
}
