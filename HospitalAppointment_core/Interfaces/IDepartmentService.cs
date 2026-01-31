using HospitalAppointment_domain.Entities;
using HospitalAppointment_core.DTOs;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces
{
    public interface IDepartmentService
    {
        Task<bool> DeleteDepartment(int departmentId);
        Task<Department> CreateDepartment(DepartmentDTO dto);
        Task<Department> UpdateDepartmentAsync(int id, DepartmentDTO dto);
        Department GetDepartmentById(int id);
    }
}
