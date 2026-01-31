using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
using HospitalAppointment_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HospitalAppointment_Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Doctor> GetDoctorsByDepartmentId(int departmentId)
        {
            return _context.Doctors
                           .Where(d => d.DepartmentId == departmentId)
                           .ToList();
        }

        public void UpdateDoctor(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
        }

        public void AddDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
        }

        public Doctor? GetByUserId(int userId)
        {
            return _context.Doctors.FirstOrDefault(d => d.UserId == userId);
        }

        public Doctor? GetById(int id)
        {
            return _context.Doctors
                           .Include(d => d.Department)
                           .FirstOrDefault(d => d.Id == id);
        }

        public void RemoveDoctor(Doctor doctor)
        {
            _context.Doctors.Remove(doctor);
        }


    }
}
