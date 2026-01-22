using HospitalAppointment_core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces
{
    public interface IPatientService
    {
        bool CreatePatient(PatientDTO dto);
    }
}
