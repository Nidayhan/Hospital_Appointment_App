using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.DTOs
{
    public class LoginRequest
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
