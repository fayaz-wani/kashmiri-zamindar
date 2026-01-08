using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Entities;

namespace KashmiriZamindar.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByGuidAsync(Guid userGuid);
        Task<User> CreateUserAsync(RegisterDto dto, string passwordHash);
        Task<bool> EmailExistsAsync(string email);
    }
}