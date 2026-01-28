using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.BusinessRules
{
    internal class PatientCanOnlyCancelOwnAppointmentRule : IBusinessRule
    {
        private readonly int _appointmentPatientId;
        private readonly int _currentUserId;

        public PatientCanOnlyCancelOwnAppointmentRule(int appointmentPatientId, int currentUserId)
        {
            _appointmentPatientId = appointmentPatientId;
            _currentUserId = currentUserId;
        }

        public void Check()
        {
            if (_appointmentPatientId != _currentUserId)
            {
                throw new Exception("Patients can only cancel their own appointments.");
            }
        }
    }
}
