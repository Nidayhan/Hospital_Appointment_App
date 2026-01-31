using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.DTOs
{
    public class DoctorUpdateDTO
    {
        // Optional user link is not changed here; updates apply to the doctor row.
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }

        public int DepartmentId { get; set; }
        public string? Specialty { get; set; }

        // Allow toggling active state; null = do not change
        public bool? IsActive { get; set; }
    }
}
