namespace FCG.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid GameId { get; set; }
    public decimal PriceAtPurchase { get; set; }

    // EF Core navigation properties
    public Game Game { get; set; } = null!;

    // Parameterless constructor for EF Core
    public OrderItem(Guid gameId, decimal priceAtPurchase)
    {
        Id = Guid.NewGuid();
        GameId = gameId;
        PriceAtPurchase = priceAtPurchase;
    }
}
