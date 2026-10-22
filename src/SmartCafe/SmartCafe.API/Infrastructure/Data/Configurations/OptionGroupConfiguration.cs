using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class OptionGroupConfiguration : IEntityTypeConfiguration<OptionGroup>
{
    public void Configure(EntityTypeBuilder<OptionGroup> builder)
    {
        builder.HasKey(og => og.Id);

        builder.Property(og => og.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(og => og.IsRequired)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(og => og.AllowMultiple)
               .IsRequired()
               .HasDefaultValue(false);
    }
}