using FCG.Domain.Entities;
using FCG.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<LibraryItem> UserGames => Set<LibraryItem>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer(_connectionString);
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Alternativamente, se preferir configurar manualmente cada entidade:
        //modelBuilder.ApplyConfiguration(new UserConfiguration());
        //modelBuilder.ApplyConfiguration(new GameConfiguration());
        //modelBuilder.ApplyConfiguration(new PromotionConfiguration());
        //modelBuilder.ApplyConfiguration(new LibraryItemConfiguration());

    }
}