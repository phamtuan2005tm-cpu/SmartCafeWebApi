using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(p => p.Description)
               .HasMaxLength(500)
               .IsRequired(false);

        builder.Property(p => p.ImageUrl)
               .HasMaxLength(500)
               .IsRequired(false);

        // Khớp với Price trong entity thực tế
        builder.Property(p => p.Price)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(p => p.StockQuantity)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(p => p.IsAvailable)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
       .IsRequired()
       .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Quan hệ 1-N với Category
        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}