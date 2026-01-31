using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
using HospitalAppointment_Infrastructure.Data;
using System.Linq;

namespace HospitalAppointment_Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Department GetDepartmentById(int departmentId)
        {
            return _context.Department.FirstOrDefault(d => d.Id == departmentId);
        }

        public void UpdateDepartment(Department department)
        {
            _context.Department.Update(department);
        }

        public void SaveDepartment(Department department)
        {
            _context.Department.Add(department);
        }
    }
}
