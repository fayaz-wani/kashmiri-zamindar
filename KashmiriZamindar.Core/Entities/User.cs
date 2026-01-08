namespace KashmiriZamindar.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public Guid UserGuid { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Order> Orders { get; set; }
    }
}