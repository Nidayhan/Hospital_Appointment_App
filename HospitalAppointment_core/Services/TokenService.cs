using HospitalAppointment_core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalAppointment_core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(int userId, string role, string name)
        {
            var jwtSection = _configuration.GetSection("Jwt");

            // fail fast with clear messages
            var secret = jwtSection["Key"] ?? throw new InvalidOperationException("Configuration 'Jwt:Key' is missing.");
            var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Configuration 'Jwt:Issuer' is missing.");
            var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Configuration 'Jwt:Audience' is missing.");

            // parse with fallback
            if (!int.TryParse(jwtSection["ExpireMinutes"], out var expireMinutes))
            {
                expireMinutes = 60; // default
            }

            // validate secret length
            if (Encoding.UTF8.GetByteCount(secret) < 32)
                throw new InvalidOperationException("Jwt:Key must be at least 32 bytes for adequate security.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim("userId", userId.ToString()),
                new Claim(ClaimTypes.Name, name ?? string.Empty),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
