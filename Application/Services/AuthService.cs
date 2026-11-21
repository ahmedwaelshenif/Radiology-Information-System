using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Settings;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUnitOfWork unitOfWork,IPasswordHasher<User> passwordHasher, IOptions<JwtSettings> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            if (user == null || !user.IsActive)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password
            );

            if (result == PasswordVerificationResult.Failed)
                return null;

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
                await _unitOfWork.SaveChangesAsync();
            }

            var token = GenerateToken(user);

            return new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }


        //Helper
        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Convert.FromBase64String(_jwtSettings.Key)
             );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _jwtSettings.DurationInMinutes
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
