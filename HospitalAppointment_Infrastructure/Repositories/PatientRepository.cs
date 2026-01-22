using HospitalAppointment_Infrastructure.Data;
using HospitalAppointment_core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool TcKimlikNoExists(string tcKimlikNo)
        {
            return _context.Patients.Any(p => p.TcKimlikNo == tcKimlikNo);
        }

        public void AddPatient(HospitalAppointment_domain.Entities.Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }
    }
}
