using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Helpers;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KashmiriZamindar.Core.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _jwtSecret = _configuration["JwtSettings:SecretKey"]
                ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required");

            if (dto.Password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            // Check if email already exists
            if (await _authRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Email already registered");

            // Hash password
            string salt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(dto.Password, salt);

            // Create user
            var user = await _authRepository.CreateUserAsync(dto, passwordHash);

            // Generate JWT token
            string token = JwtHelper.GenerateToken(user, _jwtSecret);

            return new AuthResponseDto
            {
                UserGuid = user.UserGuid,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required");

            // Get user by email
            var user = await _authRepository.GetUserByEmailAsync(dto.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated");

            // Verify password
            bool isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Generate JWT token
            string token = JwtHelper.GenerateToken(user, _jwtSecret);

            return new AuthResponseDto
            {
                UserGuid = user.UserGuid,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token
            };
        }

        public async Task<UserProfileDto> GetUserProfileAsync(Guid userGuid)
        {
            var user = await _authRepository.GetUserByGuidAsync(userGuid);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            return new UserProfileDto
            {
                UserGuid = user.UserGuid,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}