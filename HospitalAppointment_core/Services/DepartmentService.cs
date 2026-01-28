using HospitalAppointment_core.Interfaces;
using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDoctorRepository _doctorRepository;

        public DepartmentService(IDepartmentRepository departmentRepository, IDoctorRepository doctorRepository)
        {
            _departmentRepository = departmentRepository;
            _doctorRepository = doctorRepository;
        }
        public bool DeleteDepartment(int departmentId)
        {
            // Öncelikle departman var mı ve aktif mi kontrol edelim
            var department = _departmentRepository.GetDepartmentById(departmentId);
            if (department == null || !department.IsActive)
            {
                return false;
            }

            //departmana bağlı doktorları alalım
            var doctors = _doctorRepository.GetDoctorsByDepartmentId(departmentId);

            foreach (var doctor in doctors)
            {
                //doktorları pasif yapalım
                doctor.IsActive = false;
                _doctorRepository.UpdateDoctor(doctor);
            }

            //son olarak departmanı pasif yapalım
            department.IsActive = false;
            _departmentRepository.UpdateDepartment(department);

            return true;

        }
    }
}
