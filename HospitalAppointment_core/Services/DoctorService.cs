using HospitalAppointment_core.DTOs;
using HospitalAppointment_core.Interfaces;
using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
using System;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorService(IUserRepository userRepository, IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Doctor> CreateDoctorAsync(DoctorCreateDTO dto, int performedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.DepartmentId <= 0) throw new ArgumentException("DepartmentId is required.", nameof(dto));

            var doctor = new Doctor
            {
                
                // No UserId linking anymore
                Name = $"{dto.FirstName?.Trim()} {dto.LastName?.Trim()}".Trim(),
                Specialty = dto.Specialty?.Trim() ?? string.Empty,
                DepartmentId = dto.DepartmentId,
                IsActive = true
            };

            _doctorRepository.AddDoctor(doctor);

            await _unitOfWork.CommitAsync();

            return doctor;
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
        {
            return await Task.FromResult(_doctorRepository.GetById(id));
        }

        public async Task<Doctor> UpdateDoctorAsync(int id, DoctorUpdateDTO dto, int performedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.DepartmentId <= 0) throw new ArgumentException("DepartmentId is required.", nameof(dto));

            var doctor = _doctorRepository.GetById(id);
            if (doctor == null)
                throw new InvalidOperationException("Doctor not found.");

            // If doctor is linked to a user, update the user profile too
            if (doctor.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(doctor.UserId.Value);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(dto.FirstName))
                        user.FirstName = dto.FirstName.Trim();
                    if (!string.IsNullOrWhiteSpace(dto.LastName))
                        user.LastName = dto.LastName.Trim();
                    if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                        user.PhoneNumber = dto.PhoneNumber.Trim();

                    await _userRepository.UpdateAsync(user);
                    // Keep doctor.Name in sync with user full name if changed
                    doctor.Name = $"{user.FirstName} {user.LastName}".Trim();
                }
            }
            else
            {
                // No linked user — use provided name parts if present
                if (!string.IsNullOrWhiteSpace(dto.FirstName) || !string.IsNullOrWhiteSpace(dto.LastName))
                {
                    var first = dto.FirstName?.Trim() ?? string.Empty;
                    var last = dto.LastName?.Trim() ?? string.Empty;
                    var combined = $"{first} {last}".Trim();
                    if (!string.IsNullOrWhiteSpace(combined))
                        doctor.Name = combined;
                }
            }

            // update other doctor fields
            if (!string.IsNullOrWhiteSpace(dto.Specialty))
                doctor.Specialty = dto.Specialty.Trim();

            doctor.DepartmentId = dto.DepartmentId;

            if (dto.IsActive.HasValue)
                doctor.IsActive = dto.IsActive.Value;

            _doctorRepository.UpdateDoctor(doctor);

            await _unitOfWork.CommitAsync();

            return doctor;
        }

        public async Task<bool> DeleteDoctorAsync(int id, int performedByUserId)
        {
            if (id <= 0) throw new ArgumentException("Invalid doctor id.", nameof(id));

            var doctor = _doctorRepository.GetById(id);
            if (doctor == null || !doctor.IsActive)
            {
                return false;
            }

            // Deactivate doctor
            doctor.IsActive = false;
            _doctorRepository.UpdateDoctor(doctor);

            // If doctor is linked to a user, demote role to Patient unless user is Admin
            if (doctor.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(doctor.UserId.Value);
                if (user != null && !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    user.Role = "Patient";
                    await _userRepository.UpdateAsync(user);
                }
            }

            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
