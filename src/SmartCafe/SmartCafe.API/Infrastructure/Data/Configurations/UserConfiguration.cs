using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(u => u.FullName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Email)
               .HasMaxLength(150)
               .IsRequired(false);

        builder.Property(u => u.PhoneNumber)
               .HasMaxLength(20)
               .IsRequired(false);

        builder.Property(u => u.Role)
               .HasConversion<string>()
               .HasMaxLength(30)
               .IsRequired();

        builder.Property(u => u.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
       .IsRequired()
       .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}