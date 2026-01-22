using HospitalAppointment_core.DTOs;
using HospitalAppointment_core.Interfaces;
using HospitalAppointment_core.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public bool CreatePatient(PatientDTO dto)
        {
            if(_patientRepository.TcKimlikNoExists(dto.TcKimlikNo))
            {
                return false; // TC Kimlik No zaten mevcut
            }
            var patient = new HospitalAppointment_domain.Entities.Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                TcKimlikNo = dto.TcKimlikNo
            };

            _patientRepository.AddPatient(patient);

            return true;
        }
    }
}
