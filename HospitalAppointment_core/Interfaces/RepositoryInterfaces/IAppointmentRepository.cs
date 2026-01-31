using HospitalAppointment_domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces.RepositoryInterfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> IsDoctorAvailable(int doctorId, DateTime appointmentDateTime);
        Task SaveAppointment(Appointment appointment);
        Task<Appointment?> GetByIdAsync(int id);
        
        void Update(Appointment appointment);

        // Added: list appointments for a patient
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
    }
}
