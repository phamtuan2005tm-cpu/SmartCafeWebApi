using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class OrderDetailOptionConfiguration : IEntityTypeConfiguration<OrderDetailOption>
{
    public void Configure(EntityTypeBuilder<OrderDetailOption> builder)
    {
        builder.HasKey(odo => odo.Id);

        // Cấu hình các cột Snapshot
        builder.Property(odo => odo.OptionNameSnapshot)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(odo => odo.AdditionalPriceSnapshot)
               .HasPrecision(18, 2)
               .IsRequired();

        // Quan hệ 1-N với OrderDetail
        builder.HasOne(odo => odo.OrderDetail)
               .WithMany(od => od.OrderDetailOptions)
               .HasForeignKey(odo => odo.OrderDetailId)
               .OnDelete(DeleteBehavior.Cascade);

        // Quan hệ 1-N với Option gốc (Bổ sung khóa ngoại OptionId bị thiếu trước đó)
        builder.HasOne(odo => odo.Option)
               .WithMany(o => o.OrderDetailOptions)
               .HasForeignKey(odo => odo.OptionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}