using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
using HospitalAppointment_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HospitalAppointment_Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByTcAsync(string tcKimlikNo)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        // Implementation of new methods
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }
    }
}
