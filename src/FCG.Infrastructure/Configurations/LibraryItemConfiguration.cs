using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infrastructure.Configurations
{
    internal class LibraryItemConfiguration : IEntityTypeConfiguration<LibraryItem>
    {
        public void Configure(EntityTypeBuilder<LibraryItem> builder)
        {
            // Mapeamento de UserGame (Muitos-para-Muitos ou Biblioteca)

            builder.HasKey(ug => new { ug.UserId, ug.GameId });

            builder.HasOne(ug => ug.User)
             .WithMany(u => u.LibraryItems)
             .HasForeignKey(ug => ug.UserId);

            builder.HasOne(ug => ug.Game)
             .WithMany(g => g.LibraryItems)
             .HasForeignKey(ug => ug.GameId);

        }
    }
}
