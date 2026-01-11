using HospitalAppointment_core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        public bool IsDoctorAvailable(int doctorId, DateTime appointmentDateTime)
        {
            throw new NotImplementedException();
        }

        public void SaveAppointment(int patientId, int doctorId, DateTime appointmentDateTime)
        {
            throw new NotImplementedException();
        }
    }
}
