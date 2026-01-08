// ============================================
// Infrastructure/Data/AppDbContext.cs (UPDATED)
// ============================================
using KashmiriZamindar.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace KashmiriZamindar.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }  // ✅ NEW
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductGuid).IsRequired();
                entity.HasIndex(e => e.ProductGuid).IsUnique();
            });
            // Configure Order -> PaymentTransactions relationship
            modelBuilder.Entity<Order>()
                .HasMany(o => o.PaymentTransactions)
                .WithOne(pt => pt.Order)
                .HasForeignKey(pt => pt.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure decimal precision
            modelBuilder.Entity<Order>()
                .Property(o => o.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Tax)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(pt => pt.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.ToTable("AdminUsers");

                entity.HasKey(a => a.AdminId); // 🔑 PRIMARY KEY

                entity.Property(a => a.AdminGuid)
                      .IsRequired();

                entity.HasIndex(a => a.AdminGuid)
                      .IsUnique();

                entity.Property(a => a.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(a => a.FullName)
                      .HasMaxLength(200);

                entity.Property(a => a.Role)
                      .HasMaxLength(50);

                entity.Property(a => a.IsActive)
                      .HasDefaultValue(true);

                entity.Property(a => a.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

        }
    }
}

