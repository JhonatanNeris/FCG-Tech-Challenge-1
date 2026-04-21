using FCG.Domain.Enums;

namespace FCG.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;

    // Itens do pedido
    private readonly List<OrderItem> _items = new();

    // Expor os itens como somente leitura
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    protected Order() { } 

    public Order(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    public void AddItem(Guid gameId, decimal priceAtPurchase)
    {
        if (Status != OrderStatus.Pending)
            throw new Exception("Não é possível adicionar itens a um pedido que não está pendente.");

        _items.Add(new OrderItem(gameId, priceAtPurchase));
        TotalAmount += priceAtPurchase;
    }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
            throw new Exception("Apenas pedidos pendentes podem ser marcados como pagos.");

        Status = OrderStatus.Paid;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new Exception("Apenas pedidos pendentes podem ser cancelados.");

        Status = OrderStatus.Canceled;
    }



}
