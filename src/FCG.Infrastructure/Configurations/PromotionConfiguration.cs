using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace FCG.Infrastructure.Configurations;

internal class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
        builder.Property(p => p.DiscountPercentage).HasColumnType("decimal(5,2)");
        builder.Property(p => p.StartDate).IsRequired();
        builder.Property(p => p.EndDate).IsRequired();
        builder.Property(p => p.IsActive).HasDefaultValue(true);        
        builder.HasOne(p => p.Game)
         .WithMany(g => g.Promotions)
         .HasForeignKey(p => p.GameId);
    }
}
