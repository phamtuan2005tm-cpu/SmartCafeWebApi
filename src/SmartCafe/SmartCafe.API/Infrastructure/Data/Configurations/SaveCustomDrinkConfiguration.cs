using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class SavedCustomDrinkConfiguration : IEntityTypeConfiguration<SavedCustomDrink>
{
    public void Configure(EntityTypeBuilder<SavedCustomDrink> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CustomName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.SelectedOptionIdsJson)
            .IsRequired()
            .HasDefaultValue("[]");

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Nếu món gốc bị xóa thì tự động dọn công thức liên quan
        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}