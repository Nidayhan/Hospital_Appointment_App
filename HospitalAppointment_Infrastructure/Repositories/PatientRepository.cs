using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
using HospitalAppointment_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

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

        public void AddPatient(Patient patient)
        {
            _context.Patients.Add(patient);
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Patient?> GetByTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return null;

            return await _context.Patients.FirstOrDefaultAsync(p => p.TcKimlikNo == tcKimlikNo);
        }
    }
}
