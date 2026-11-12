using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Infrastructure.Data.Configurations;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.HasKey(wt => wt.Id);

        builder.Property(wt => wt.Type)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(wt => wt.Description)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(wt => wt.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(wt => wt.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wt => wt.Order)
            .WithMany()
            .HasForeignKey(wt => wt.OrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}