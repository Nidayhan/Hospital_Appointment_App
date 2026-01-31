using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string TcKimlikNo { get; set; } = string.Empty;

        // Added for activation/state tracking
        public bool IsActive { get; set; } = true;
    }
}
