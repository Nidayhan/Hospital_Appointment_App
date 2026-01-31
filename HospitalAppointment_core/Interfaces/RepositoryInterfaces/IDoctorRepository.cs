using HospitalAppointment_domain.Entities;
using System.Collections.Generic;

namespace HospitalAppointment_core.Interfaces.RepositoryInterfaces
{
    public interface IDoctorRepository
    {
        List<Doctor> GetDoctorsByDepartmentId(int departmentId);
        void UpdateDoctor(Doctor doctor);
        void AddDoctor(Doctor doctor);

        // Return the Doctor record linked to a user (nullable if none).
        Doctor? GetByUserId(int userId);

        // fetch Doctor by primary key Id
        Doctor? GetById(int id);

        // remove a doctor entity (used for hard delete if ever needed)
        void RemoveDoctor(Doctor doctor);
    }
}
