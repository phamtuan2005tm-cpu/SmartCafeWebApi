using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.TableName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.QrCodeToken)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(20);

        // Đảm bảo số bàn và mã QR là duy nhất
        builder.HasIndex(t => t.TableNumber).IsUnique();
        builder.HasIndex(t => t.QrCodeToken).IsUnique();
    }
}