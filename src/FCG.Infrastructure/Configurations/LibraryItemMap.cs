using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infrastructure.Configurations;

internal class LibraryItemMap() : IEntityTypeConfiguration<LibraryItem>
{
    public void Configure(EntityTypeBuilder<LibraryItem> builder)
    {
        builder.HasKey(ug => new { ug.UserId, ug.GameId });

        builder.HasOne(ug => ug.User)
            .WithMany(u => u.LibraryItems)
            .HasForeignKey(ug => ug.UserId);

        builder.HasOne(ug => ug.Game)
            .WithMany(g => g.LibraryItems)
            .HasForeignKey(ug => ug.GameId);
    }
}
