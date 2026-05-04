namespace FCG.Domain.Entities;

public class OrderItem(Guid gameId, decimal priceAtPurchase)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid GameId { get; set; } = gameId;
    public decimal PriceAtPurchase { get; set; } = priceAtPurchase;
    public Game Game { get; set; } = null!;

    protected OrderItem() : this(Guid.Empty, 0)
    {
    }
}
