using HospitalAppointment_domain.Entities;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByTcAsync(string tcKimlikNo);
        Task AddAsync(User user);

        // Added to support profile read/update
        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(User user);
    }
}
