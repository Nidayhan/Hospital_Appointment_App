using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Services
{
    public interface IAppointmentService
    {
        public void CreateAppointment(int patientId, int doctorId, DateTime dateTime);
    }
}
