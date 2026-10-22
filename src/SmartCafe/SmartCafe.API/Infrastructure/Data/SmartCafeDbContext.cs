using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Data;
using SmartCafe.API.Domain.Entities;
using System.Reflection;

namespace SmartCafe.API.Infrastructure.Data
{
    public class SmartCafeDbContext : DbContext 
    {
        public SmartCafeDbContext(DbContextOptions<SmartCafeDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

        // Nhóm tùy chọn và cấu hình
        public DbSet<OptionGroup> OptionGroups => Set<OptionGroup>();
        public DbSet<Option> Options => Set<Option>();
        public DbSet<ProductOptionGroup> ProductOptionGroups => Set<ProductOptionGroup>();
        public DbSet<OrderDetailOption> OrderDetailOptions => Set<OrderDetailOption>();
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Tự động áp dụng tất cả các file Configuration (IEntityTypeConfiguration<T>)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // 2. Cấu hình Precision chuẩn cho toàn bộ kiểu Decimal (numeric(18,2)) theo NFR-DATA-01
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("numeric(18,2)");
            }

            // 3. Cấu hình Khóa chính hợp phần cho bảng trung gian ProductOptionGroup
            modelBuilder.Entity<ProductOptionGroup>()
                .HasKey(pog => new { pog.ProductId, pog.OptionGroupId });

            // 4. Gọi Data Seeding từ DbInitializer
            DbInitializer.SeedData(modelBuilder);
        }
    }
}
