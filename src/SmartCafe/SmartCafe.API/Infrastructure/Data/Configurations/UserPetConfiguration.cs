using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class UserPetConfiguration : IEntityTypeConfiguration<UserPet>
{
    public void Configure(EntityTypeBuilder<UserPet> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.UserId).IsUnique(); // Mỗi User tối đa 1 Pet
        builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
        builder.Property(p => p.PetType).IsRequired().HasMaxLength(30);

        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<UserPet>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}