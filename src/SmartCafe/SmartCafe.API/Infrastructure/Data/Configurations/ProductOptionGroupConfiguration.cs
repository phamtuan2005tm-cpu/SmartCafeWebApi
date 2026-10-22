using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class ProductOptionGroupConfiguration : IEntityTypeConfiguration<ProductOptionGroup>
{
    public void Configure(EntityTypeBuilder<ProductOptionGroup> builder)
    {
        // Khóa chính kép gồm 2 Guid
        builder.HasKey(pog => new { pog.ProductId, pog.OptionGroupId });

        builder.HasOne(pog => pog.Product)
               .WithMany(p => p.ProductOptionGroups)
               .HasForeignKey(pog => pog.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pog => pog.OptionGroup)
               .WithMany(og => og.ProductOptionGroups)
               .HasForeignKey(pog => pog.OptionGroupId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}