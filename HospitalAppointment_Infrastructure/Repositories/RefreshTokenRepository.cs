using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
using HospitalAppointment_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HospitalAppointment_Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.Set<RefreshToken>().AddAsync(token);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.Set<RefreshToken>().FirstOrDefaultAsync(t => t.Token == token);
        }

        public void Update(RefreshToken token)
        {
            _context.Set<RefreshToken>().Update(token);
        }
    }
}
