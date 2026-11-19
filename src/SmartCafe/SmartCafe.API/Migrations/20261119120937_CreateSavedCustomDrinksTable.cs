using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCafe.API.Migrations
{
    /// <inheritdoc />
    public partial class CreateSavedCustomDrinksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedCustomDrinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SelectedOptionIdsJson = table.Column<string>(type: "text", nullable: false, defaultValue: "[]"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedCustomDrinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedCustomDrinks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedCustomDrinks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedCustomDrinks_ProductId",
                table: "SavedCustomDrinks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCustomDrinks_UserId",
                table: "SavedCustomDrinks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedCustomDrinks");
        }
    }
}
