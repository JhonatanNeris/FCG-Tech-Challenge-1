using FCG.Domain.Common;
using FCG.Domain.Enums;

namespace FCG.Domain.Entities;

public class Order(Guid userId)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = userId;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public User User { get; set; } = null!;

    private readonly List<OrderItem> _items = new();

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    protected Order() : this(Guid.Empty)
    {
    }

    public Result AddItem(Guid gameId, decimal priceAtPurchase)
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Failure(Error.Validation("Orders.InvalidStatus", "Nao e possivel adicionar itens a um pedido que nao esta pendente."));
        }

        _items.Add(new OrderItem(gameId, priceAtPurchase));
        TotalAmount += priceAtPurchase;
        return Result.Success();
    }

    public Result MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Failure(Error.Validation("Orders.InvalidStatus", "Apenas pedidos pendentes podem ser marcados como pagos."));
        }

        Status = OrderStatus.Paid;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Failure(Error.Validation("Orders.InvalidStatus", "Apenas pedidos pendentes podem ser cancelados."));
        }

        Status = OrderStatus.Canceled;
        return Result.Success();
    }
}
