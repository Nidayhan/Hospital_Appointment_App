using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.DTOs
{
    public class DoctorCreateDTO
    {

        // Doctor profile fields
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public string Specialty { get; set; } = string.Empty;
    }
}
