using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces
{
    public interface IAppointmentService
    {
        Task CancelAppointment(int appointmentId, int currentUserId);
        public Task CreateAppointment(int patientId, int doctorId, DateTime appointmentDateTime);
    }
}
