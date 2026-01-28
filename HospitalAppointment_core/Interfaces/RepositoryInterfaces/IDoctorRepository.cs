using HospitalAppointment_domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces.RepositoryInterfaces
{
    public interface IDoctorRepository
    {
        List<Doctor> GetDoctorsByDepartmentId (int departmentId);
        void UpdateDoctor(Doctor doctor);
    }
}
