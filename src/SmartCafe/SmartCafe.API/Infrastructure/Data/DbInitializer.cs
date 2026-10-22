using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace SmartCafe.API.Data;

public static class DbInitializer
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        // =============================================================
        // 1. SEED USERS (Admin, Staff, Customer)
        // =============================================================
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var staffId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var customerId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminId,
                Email = "admin@smartcafe.com",
                PasswordHash = HashPassword("Admin@123"),
                FullName = "System Administrator",
                Role = "Admin",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = staffId,
                Email = "staff@smartcafe.com",
                PasswordHash = HashPassword("Staff@123"),
                FullName = "Barista Staff",
                Role = "Staff",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = customerId,
                Email = "customer@gmail.com",
                PasswordHash = HashPassword("Customer@123"),
                FullName = "Nguyen Van A",
                Role = "Customer",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // =============================================================
        // 2. SEED CATEGORIES
        // =============================================================
        var catCoffeeId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
        var catTeaId = Guid.Parse("a2222222-2222-2222-2222-222222222222");

        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = catCoffeeId,
                Name = "Cà Phê",
                Description = "Các món cà phê đậm đà chuẩn vị Việt và Ý",
                DisplayOrder = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = catTeaId,
                Name = "Trà & Trái Cây",
                Description = "Trà thanh mát kết hợp trái cây tươi nhiệt đới",
                DisplayOrder = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // =============================================================
        // 3. SEED PRODUCTS
        // =============================================================
        var prodBacSiu = Guid.Parse("b1111111-1111-1111-1111-111111111111");
        var prodPeachTea = Guid.Parse("b2222222-2222-2222-2222-222222222222");

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = prodBacSiu,
                CategoryId = catCoffeeId,
                Name = "Bạc Sỉu Sữa Dừa",
                Description = "Cà phê espresso hòa quyện cùng sữa đặc và nước cốt dừa béo ngậy.",
                Price = 35000m,
                ImageUrl = "/images/products/bac-siu.png",
                StockQuantity = 100,
                IsAvailable = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = prodPeachTea,
                CategoryId = catTeaId,
                Name = "Trà Đào Cam Sả",
                Description = "Trà đen vị đào thơm mát, kết hợp vị chua nhẹ của cam tươi và hương sả.",
                Price = 42000m,
                ImageUrl = "/images/products/tra-dao.png",
                StockQuantity = 50,
                IsAvailable = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // =============================================================
        // 4. SEED OPTION GROUPS & OPTIONS
        // =============================================================
        var grpSugar = Guid.Parse("c1111111-1111-1111-1111-111111111111");
        var grpIce = Guid.Parse("c2222222-2222-2222-2222-222222222222");

        modelBuilder.Entity<OptionGroup>().HasData(
            new OptionGroup { Id = grpSugar, Name = "Mức Đường", InputType = "Single" },
            new OptionGroup { Id = grpIce, Name = "Mức Đá", InputType = "Single" }
        );

        modelBuilder.Entity<Option>().HasData(
            new Option { Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"), OptionGroupId = grpSugar, Name = "100% Đường", AdditionalPrice = 0m, DisplayOrder = 1 },
            new Option { Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"), OptionGroupId = grpSugar, Name = "50% Đường", AdditionalPrice = 0m, DisplayOrder = 2 },
            new Option { Id = Guid.Parse("d3333333-3333-3333-3333-333333333333"), OptionGroupId = grpIce, Name = "100% Đá", AdditionalPrice = 0m, DisplayOrder = 1 },
            new Option { Id = Guid.Parse("d4444444-4444-4444-4444-444444444444"), OptionGroupId = grpIce, Name = "50% Đá", AdditionalPrice = 0m, DisplayOrder = 2 }
        );

        // =============================================================
        // 5. SEED PRODUCT - OPTION GROUPS (Junction Table)
        // =============================================================
        modelBuilder.Entity<ProductOptionGroup>().HasData(
            new ProductOptionGroup { ProductId = prodBacSiu, OptionGroupId = grpSugar },
            new ProductOptionGroup { ProductId = prodBacSiu, OptionGroupId = grpIce },
            new ProductOptionGroup { ProductId = prodPeachTea, OptionGroupId = grpSugar },
            new ProductOptionGroup { ProductId = prodPeachTea, OptionGroupId = grpIce }
        );
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}