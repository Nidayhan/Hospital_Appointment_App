using HospitalAppointment_core.DTOs;
using HospitalAppointment_core.Interfaces;
using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly PasswordHasher<object> _passwordHasher = new();

        public AuthService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IPatientRepository patientRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _patientRepository = patientRepository;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // Basic input validation
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");

            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                throw new ArgumentException("FirstName and LastName are required.");

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                throw new ArgumentException("PhoneNumber is required.");

            if (string.IsNullOrWhiteSpace(request.TcKimlikNo))
                throw new ArgumentException("TcKimlikNo is required.");

            if (request.TcKimlikNo.Length != 11 || !request.TcKimlikNo.All(char.IsDigit))
                throw new ArgumentException("TcKimlikNo must be 11 digits.");

            if (!IsValidTcKimlik(request.TcKimlikNo))
                throw new ArgumentException("Invalid TcKimlikNo format.");

            // Uniqueness checks (by TC)
            var existingTc = await _userRepository.GetByTcAsync(request.TcKimlikNo);
            if (existingTc != null)
                throw new InvalidOperationException("TcKimlikNo already registered.");

            var user = new User
            {
                Role = "Patient",
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                TcKimlikNo = request.TcKimlikNo,
                PhoneNumber = request.PhoneNumber.Trim(),
                IsActive = true
            };

            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(null!, request.Password);

            // add user and patient before committing so both persist in same transaction
            await _userRepository.AddAsync(user);

            var patientEntity = new Patient
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                TcKimlikNo = user.TcKimlikNo,
                IsActive = true
            };
            _patientRepository.AddPatient(patientEntity);

            // commit so user.Id is generated
            await _unitOfWork.CommitAsync();

            var fullName = $"{user.FirstName} {user.LastName}";
            var accessToken = _tokenService.CreateToken(user.Id, user.Role, fullName);

            // create refresh token
            var refreshToken = CreateRefreshToken();
            var refreshEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepository.AddAsync(refreshEntity);
            await _unitOfWork.CommitAsync();

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(60) // keep in sync with Jwt:ExpireMinutes if needed
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.TcKimlikNo) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("TcKimlikNo and password are required.");

            var user = await _userRepository.GetByTcAsync(request.TcKimlikNo);
            if (user == null) throw new InvalidOperationException("Invalid credentials.");

            if (!user.IsActive) throw new InvalidOperationException("User is inactive.");

            var verification = _passwordHasher.VerifyHashedPassword(null!, user.PasswordHash, request.Password);
            if (verification == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Invalid credentials.");

            var fullName = $"{user.FirstName} {user.LastName}";
            var accessToken = _tokenService.CreateToken(user.Id, user.Role, fullName);

            // create refresh token
            var refreshToken = CreateRefreshToken();
            var refreshEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepository.AddAsync(refreshEntity);
            await _unitOfWork.CommitAsync();

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(60) // sync with JWT expire if used
            };
        }

        public async Task LogoutAsync(string refreshToken, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var tokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (tokenEntity == null)
                return; // nothing to do

            if (tokenEntity.UserId != currentUserId)
                throw new InvalidOperationException("Refresh token does not belong to the current user.");

            // revoke
            tokenEntity.RevokedAt = DateTime.UtcNow;
            _refreshTokenRepository.Update(tokenEntity);
            await _unitOfWork.CommitAsync();
        }

        private bool IsValidTcKimlik(string tc)
        {
            if (tc.Length != 11 || !tc.All(char.IsDigit)) return false;
            var digits = tc.Select(c => c - '0').ToArray();
            if (digits[0] == 0) return false;
            int sumOdd = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int sumEven = digits[1] + digits[3] + digits[5] + digits[7];
            int digit10 = ((sumOdd * 7) - sumEven) % 10;
            int digit11 = digits.Take(10).Sum() % 10;
            return digit10 == digits[9] && digit11 == digits[10];
        }

        // Create secure random refresh token
        private static string CreateRefreshToken()
        {
            var randomBytes = new byte[64];
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        // small guards to avoid accidental renames by refactor tools in this snippet
        private static string _token_service_create_token_guard(ITokenService svc, int id, string role, string name) =>
            svc.CreateToken(id, role, name);

        private static PasswordVerificationResult _password_hasher_verify_guard(PasswordHasher<object> hasher, string hash, string password) =>
            hasher.VerifyHashedPassword(null!, hash, password);
    }
}
