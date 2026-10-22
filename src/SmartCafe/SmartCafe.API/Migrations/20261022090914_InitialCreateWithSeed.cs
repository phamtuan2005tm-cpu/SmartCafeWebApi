using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartCafe.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Các món cà phê đậm đà chuẩn vị Việt và Ý", 1, "Cà Phê" },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trà thanh mát kết hợp trái cây tươi nhiệt đới", 2, "Trà & Trái Cây" }
                });

            migrationBuilder.InsertData(
                table: "OptionGroups",
                columns: new[] { "Id", "InputType", "Name" },
                values: new object[,]
                {
                    { new Guid("c1111111-1111-1111-1111-111111111111"), "Single", "Mức Đường" },
                    { new Guid("c2222222-2222-2222-2222-222222222222"), "Single", "Mức Đá" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "PhoneNumber", "Role" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@smartcafe.com", "System Administrator", true, "6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc=", "", "Admin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff@smartcafe.com", "Barista Staff", true, "39SPNjOKo2Io67niBLumtOGNsLYj4lxFiQHtyDH7GOk=", "", "Staff" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer@gmail.com", "Nguyen Van A", true, "mOxlSo3yj48PjwIiBIPUaRa4UBfeC3TY7HVcKMuFOag=", "", "Customer" }
                });

            migrationBuilder.InsertData(
                table: "Options",
                columns: new[] { "Id", "AdditionalPrice", "DisplayOrder", "ExtraPrice", "IsAvailable", "LayerImageUrl", "Name", "OptionGroupId", "QuantityValue", "Unit" },
                values: new object[,]
                {
                    { new Guid("d1111111-1111-1111-1111-111111111111"), 0m, 1, 0m, true, null, "100% Đường", new Guid("c1111111-1111-1111-1111-111111111111"), 0m, "ml" },
                    { new Guid("d2222222-2222-2222-2222-222222222222"), 0m, 2, 0m, true, null, "50% Đường", new Guid("c1111111-1111-1111-1111-111111111111"), 0m, "ml" },
                    { new Guid("d3333333-3333-3333-3333-333333333333"), 0m, 1, 0m, true, null, "100% Đá", new Guid("c2222222-2222-2222-2222-222222222222"), 0m, "ml" },
                    { new Guid("d4444444-4444-4444-4444-444444444444"), 0m, 2, 0m, true, null, "50% Đá", new Guid("c2222222-2222-2222-2222-222222222222"), 0m, "ml" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsAvailable", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { new Guid("b1111111-1111-1111-1111-111111111111"), new Guid("a1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cà phê espresso hòa quyện cùng sữa đặc và nước cốt dừa béo ngậy.", "/images/products/bac-siu.png", true, "Bạc Sỉu Sữa Dừa", 35000m, 100 },
                    { new Guid("b2222222-2222-2222-2222-222222222222"), new Guid("a2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trà đen vị đào thơm mát, kết hợp vị chua nhẹ của cam tươi và hương sả.", "/images/products/tra-dao.png", true, "Trà Đào Cam Sả", 42000m, 50 }
                });

            migrationBuilder.InsertData(
                table: "ProductOptionGroups",
                columns: new[] { "OptionGroupId", "ProductId" },
                values: new object[,]
                {
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("b1111111-1111-1111-1111-111111111111") },
                    { new Guid("c2222222-2222-2222-2222-222222222222"), new Guid("b1111111-1111-1111-1111-111111111111") },
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("b2222222-2222-2222-2222-222222222222") },
                    { new Guid("c2222222-2222-2222-2222-222222222222"), new Guid("b2222222-2222-2222-2222-222222222222") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Options",
                keyColumn: "Id",
                keyValue: new Guid("d1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Options",
                keyColumn: "Id",
                keyValue: new Guid("d2222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Options",
                keyColumn: "Id",
                keyValue: new Guid("d3333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Options",
                keyColumn: "Id",
                keyValue: new Guid("d4444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "ProductOptionGroups",
                keyColumns: new[] { "OptionGroupId", "ProductId" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("b1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "ProductOptionGroups",
                keyColumns: new[] { "OptionGroupId", "ProductId" },
                keyValues: new object[] { new Guid("c2222222-2222-2222-2222-222222222222"), new Guid("b1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "ProductOptionGroups",
                keyColumns: new[] { "OptionGroupId", "ProductId" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("b2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "ProductOptionGroups",
                keyColumns: new[] { "OptionGroupId", "ProductId" },
                keyValues: new object[] { new Guid("c2222222-2222-2222-2222-222222222222"), new Guid("b2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "OptionGroups",
                keyColumn: "Id",
                keyValue: new Guid("c1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "OptionGroups",
                keyColumn: "Id",
                keyValue: new Guid("c2222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222222"));
        }
    }
}
