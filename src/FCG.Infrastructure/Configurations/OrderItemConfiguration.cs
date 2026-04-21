using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.PriceAtPurchase).HasColumnType("decimal(18,2)");

        builder.HasOne(oi => oi.Game)
            .WithMany()
            .HasForeignKey(oi => oi.GameId);
    }
}