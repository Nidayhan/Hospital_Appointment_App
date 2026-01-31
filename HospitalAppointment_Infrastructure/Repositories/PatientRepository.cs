using HospitalAppointment_Infrastructure.Data;
using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
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
            // Do NOT call SaveChanges here. UnitOfWork.CommitAsync() will persist.
            _context.Patients.Add(patient);
        }
    }
}
