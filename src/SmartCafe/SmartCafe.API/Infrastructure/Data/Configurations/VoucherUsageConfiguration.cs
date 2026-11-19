using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class VoucherUsageConfiguration : IEntityTypeConfiguration<VoucherUsage>
{
    public void Configure(EntityTypeBuilder<VoucherUsage> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => new { u.VoucherId, u.UserId, u.OrderId }).IsUnique();
        builder.Property(u => u.DiscountApplied).HasPrecision(18, 2);

        builder.HasOne(u => u.Voucher)
            .WithMany(v => v.Usages)
            .HasForeignKey(u => u.VoucherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Order)
            .WithMany()
            .HasForeignKey(u => u.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}