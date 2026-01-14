using HospitalAppointment_domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces
{
    public interface IAppointmentRepository
    {
        bool IsDoctorAvailable(int doctorId, DateTime appointmentDateTime);
        void SaveAppointment(Appointment appointment);
    }
}
