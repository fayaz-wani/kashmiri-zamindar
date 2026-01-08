using System.Security.Cryptography;
using System.Text;

namespace KashmiriZamindar.Core.Helpers
{
    public static class PasswordHelper
    {
        



        // Generate a salt
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Hash password with salt using PBKDF2
        public static string HashPassword(string password, string salt)
        {

            byte[] saltBytes = Convert.FromBase64String(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);

                // Combine salt and hash
                byte[] hashBytes = new byte[saltBytes.Length + hash.Length];
                Array.Copy(saltBytes, 0, hashBytes, 0, saltBytes.Length);
                Array.Copy(hash, 0, hashBytes, saltBytes.Length, hash.Length);

                return Convert.ToBase64String(hashBytes);
            }
        }

        // Verify password
        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(storedHash);

                // Extract salt (first 32 bytes)
                byte[] salt = new byte[32];
                Array.Copy(hashBytes, 0, salt, 0, 32);

                // Hash the input password with the extracted salt
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);

                    // Compare the computed hash with stored hash
                    for (int i = 0; i < 32; i++)
                    {
                        if (hashBytes[i + 32] != hash[i])
                            return false;
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // Alternative: Simpler hash for development (use PBKDF2 above for production)
        public static string SimpleHashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
   
        // Verify simple hash
        public static bool VerifySimplePassword(string password, string storedHash)
        {
            string computedHash = SimpleHashPassword(password);
            return computedHash == storedHash;
        }
    }
}