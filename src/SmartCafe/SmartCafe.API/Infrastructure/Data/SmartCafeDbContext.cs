using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
