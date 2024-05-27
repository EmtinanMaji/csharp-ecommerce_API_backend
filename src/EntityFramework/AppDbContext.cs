using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //---------------- Order (Atheer) -------------------
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.Property(o => o.OrderId).IsRequired().ValueGeneratedOnAdd();
                entity.Property(o => o.OrderStatus).IsRequired();
                entity.Property(o => o.OrderTotal).IsRequired();
                entity.Property(o => o.OrderDate).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            });

            //----------------- Product (Nouir) --------------------
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.ProductId).HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Slug).HasMaxLength(100);
                entity.Property(p => p.ImageUrl);
                entity.Property(p => p.Description).HasMaxLength(300);;
                entity.Property(p => p.Price).IsRequired();
                entity.Property(p => p.Quantity);
                entity.Property(p => p.Sold);
                entity.Property(p => p.Shipping)
                    .HasColumnName("shipping");
                entity.Property(p => p.CategoryId)
                    .HasColumnName("category_id");
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            //---------------- Category (Emtinan) ------------------
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.CategoryId).IsRequired().ValueGeneratedOnAdd();
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(c => c.Name).IsUnique();
                entity.Property(c => c.Description).HasDefaultValue(string.Empty);
            });

            // ----------------- User (Enas) -------------------
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserId).HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Address).HasMaxLength(255);
                entity.Property(u => u.IsAdmin).HasDefaultValue(false);
                entity.Property(u => u.IsBanned).HasDefaultValue(false);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // one-to-many relationship between User & order
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            // one-to-many relationship between Product & Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            //many-to-many relationship between Product & order
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.ProductId });

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

        }
    }
}