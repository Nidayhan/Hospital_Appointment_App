using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalAppointment_core.DTOs;

namespace HospitalAppointment_core.Interfaces
{
    public interface IAppointmentService
    {
        Task CancelAppointment(int appointmentId, int currentUserId);
        Task CreateAppointment(int patientId, int doctorId, DateTime appointmentDateTime);
        Task<IEnumerable<AppointmentResponseDTO>> GetAppointmentsByPatientAsync(int patientId);
    }
}
