namespace KashmiriZamindar.Core.Entities
{
    public class AdminUser
    {
        public int AdminId { get; set; }
        public Guid AdminGuid { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } = "Admin"; // Admin, SuperAdmin
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }
}