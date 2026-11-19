using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.HasKey(v => v.Id);
        builder.HasIndex(v => v.Code).IsUnique(); // Mã Code không được trùng nhau
        builder.Property(v => v.Code).IsRequired().HasMaxLength(50);
        builder.Property(v => v.Title).IsRequired().HasMaxLength(150);
        builder.Property(v => v.DiscountType).IsRequired().HasMaxLength(20);
        builder.Property(v => v.DiscountValue).HasPrecision(18, 2);
        builder.Property(v => v.MaxDiscountAmount).HasPrecision(18, 2);
        builder.Property(v => v.MinOrderAmount).HasPrecision(18, 2);

        builder.HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}