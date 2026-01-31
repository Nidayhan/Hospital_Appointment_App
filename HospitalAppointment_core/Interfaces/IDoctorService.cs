using HospitalAppointment_core.DTOs;
using HospitalAppointment_domain.Entities;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces
{
    public interface IDoctorService
    {
        Task<Doctor> CreateDoctorAsync(DoctorCreateDTO dto, int performedByUserId);

        // fetch a doctor by the doctor's primary key Id
        Task<Doctor?> GetDoctorByIdAsync(int id);

        // update existing doctor by table Id
        Task<Doctor> UpdateDoctorAsync(int id, DoctorUpdateDTO dto, int performedByUserId);

        // soft-delete (deactivate) doctor by table Id
        Task<bool> DeleteDoctorAsync(int id, int performedByUserId);
    }
}
