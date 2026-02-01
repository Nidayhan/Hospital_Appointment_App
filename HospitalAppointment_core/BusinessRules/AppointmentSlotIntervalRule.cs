using System;

namespace HospitalAppointment_core.BusinessRules
{
    public class AppointmentSlotIntervalRule : IBusinessRule
    {
        private readonly DateTime _appointmentDateTime;
        private readonly int _slotMinutes;

        public AppointmentSlotIntervalRule(DateTime appointmentDateTime, int slotMinutes)
        {
            _appointmentDateTime = appointmentDateTime;
            _slotMinutes = slotMinutes;
        }

        public void Check()
        {
            if (_slotMinutes <= 0) return;

            if (_appointmentDateTime.Second != 0 || _appointmentDateTime.Millisecond != 0)
                throw new Exception($"Appointment seconds and milliseconds must be zero and minutes aligned to {_slotMinutes}.");

            if (_appointmentDateTime.Minute % _slotMinutes != 0)
                throw new Exception($"Appointment minutes must align to {_slotMinutes}-minute slots (e.g. 09:00, 09:30).");
        }
    }
}
