using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.BusinessRules
{
    public class CannotCancelPastAppointmentRule : IBusinessRule
    {
        private readonly DateTime _appointmentDate;
        public CannotCancelPastAppointmentRule(DateTime appointmentDate)
        {
            _appointmentDate = appointmentDate;
        }
        public void Check()
        {
            if (_appointmentDate < DateTime.Now)
            {
                throw new Exception("Cannot cancel past appointments.");
            }
        }
    }
}
