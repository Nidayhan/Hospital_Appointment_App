using HospitalAppointment_domain.Entities;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces.RepositoryInterfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
        void Update(RefreshToken token);

    }
}
