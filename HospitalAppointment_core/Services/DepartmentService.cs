using HospitalAppointment_core.Interfaces;
using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_core.DTOs;
using HospitalAppointment_domain.Entities;
using System;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IDepartmentRepository departmentRepository, IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Department> CreateDepartment(DepartmentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Department name is required.", nameof(dto));

            var department = new Department
            {
                Name = dto.Name.Trim(),
                IsActive = true
            };

            _departmentRepository.SaveDepartment(department);
            await _unitOfWork.CommitAsync();

            return department;
        }

        public Department GetDepartmentById(int id)
        {
            return _departmentRepository.GetDepartmentById(id);
        }

        public async Task<Department> UpdateDepartmentAsync(int id, DepartmentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Department name is required.", nameof(dto));

            var department = _departmentRepository.GetDepartmentById(id);
            if (department == null)
                throw new InvalidOperationException("Department not found.");

            department.Name = dto.Name.Trim();
            _departmentRepository.UpdateDepartment(department);

            await _unitOfWork.CommitAsync();

            return department;
        }

        public async Task<bool> DeleteDepartment(int departmentId)
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

            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
