using HospitalAppointment_domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.BusinessRules
{
    public class CannotModifyCompletedAppointmentRule : IBusinessRule
    {
        private readonly AppointmentStatus _status;

        public CannotModifyCompletedAppointmentRule(AppointmentStatus status)
        {
            _status = status;
        }

        public void Check()
        {
            if (_status == AppointmentStatus.Completed)
            {
                throw new Exception("Cannot modify a completed appointment.");
            }
        }
    }
}
