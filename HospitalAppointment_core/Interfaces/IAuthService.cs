using HospitalAppointment_core.DTOs;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);

        // Logout by refresh token (must belong to current user)
        Task LogoutAsync(string refreshToken, int currentUserId);
    }
}
