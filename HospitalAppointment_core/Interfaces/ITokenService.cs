using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace HospitalAppointment_core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(int userId, string role, string name);
    }
}
