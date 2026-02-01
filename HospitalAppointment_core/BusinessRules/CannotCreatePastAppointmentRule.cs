using System;

namespace HospitalAppointment_core.BusinessRules
{
    public class CannotCreatePastAppointmentRule : IBusinessRule
    {
        private readonly DateTime _appointmentDate;

        public CannotCreatePastAppointmentRule(DateTime appointmentDate)
        {
            _appointmentDate = appointmentDate;
        }

        public void Check()
        {
            // Compare in UTC to avoid timezone mismatches.
            var nowUtc = DateTime.UtcNow;
            if (_appointmentDate.ToUniversalTime() <= nowUtc)
            {
                throw new Exception("Cannot create an appointment in the past.");
            }
        }
    }
}
