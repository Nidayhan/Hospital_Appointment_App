using System;

namespace HospitalAppointment_core.BusinessRules
{
    public class AppointmentWithinWorkingHoursRule : IBusinessRule
    {
        private readonly TimeSpan _appointmentTime;
        private readonly TimeSpan _workStart;
        private readonly TimeSpan _workEnd;

        public AppointmentWithinWorkingHoursRule(TimeSpan appointmentTime, TimeSpan workStart, TimeSpan workEnd)
        {
            _appointmentTime = appointmentTime;
            _workStart = workStart;
            _workEnd = workEnd;
        }

        public void Check()
        {
            // allow appointment at exactly start or exactly end
            if (_appointmentTime < _workStart || _appointmentTime > _workEnd)
            {
                throw new Exception($"Appointment must be between {_workStart:hh\\:mm} and {_workEnd:hh\\:mm}.");
            }
        }
    }
}
