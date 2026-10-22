using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class OptionConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
               .IsRequired()
               .HasMaxLength(100);

        // Các trường giá tiền và giá trị định lượng
        builder.Property(o => o.AdditionalPrice)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(o => o.QuantityValue)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(o => o.ExtraPrice)
               .HasPrecision(18, 2)
               .IsRequired();

        // Đơn vị tính (ml, g,...)
        builder.Property(o => o.Unit)
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(o => o.LayerImageUrl)
               .HasMaxLength(500)
               .IsRequired(false);

        builder.Property(o => o.DisplayOrder)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(o => o.IsAvailable)
               .IsRequired()
               .HasDefaultValue(true);

        // Quan hệ 1-N với OptionGroup
        builder.HasOne(o => o.OptionGroup)
               .WithMany(og => og.Options)
               .HasForeignKey(o => o.OptionGroupId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}