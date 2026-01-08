using System.Data;
using Microsoft.Data.SqlClient;
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KashmiriZamindar.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetUserByEmail", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new User
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                UserGuid = reader.GetGuid(reader.GetOrdinal("UserGuid")),
                Email = reader["Email"].ToString(),
                PasswordHash = reader["PasswordHash"].ToString(),
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"].ToString(),
                PhoneNumber = reader["PhoneNumber"]?.ToString(),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        public async Task<User?> GetUserByGuidAsync(Guid userGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetUserByGuid", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserGuid", userGuid);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new User
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                UserGuid = reader.GetGuid(reader.GetOrdinal("UserGuid")),
                Email = reader["Email"].ToString(),
                PasswordHash = reader["PasswordHash"].ToString(),
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"].ToString(),
                PhoneNumber = reader["PhoneNumber"]?.ToString(),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        public async Task<User> CreateUserAsync(RegisterDto dto, string passwordHash)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_CreateUser", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", dto.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@FirstName", dto.FirstName);
            cmd.Parameters.AddWithValue("@LastName", dto.LastName);
            cmd.Parameters.AddWithValue("@PhoneNumber", dto.PhoneNumber ?? (object)DBNull.Value);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new Exception("Failed to create user");

            return new User
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                UserGuid = reader.GetGuid(reader.GetOrdinal("UserGuid")),
                Email = reader["Email"].ToString(),
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"].ToString(),
                PhoneNumber = reader["PhoneNumber"]?.ToString(),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                IsActive = true
            };
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_CheckEmailExists", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            await con.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }
    }
}