using HospitalAppointment_domain.Entities;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces.RepositoryInterfaces
{
    public interface IPatientRepository
    {
        bool TcKimlikNoExists(string tcKimlikNo);
        void AddPatient(Patient patient);

        // fetch patient by id (async)
        Task<Patient?> GetByIdAsync(int id);

        // fetch patient by TcKimlikNo (async) - added to resolve Patients from User records
        Task<Patient?> GetByTcAsync(string tcKimlikNo);
    }
}
