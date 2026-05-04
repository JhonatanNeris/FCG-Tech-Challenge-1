using FCG.Domain.Common;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<Result> AddAsync(Order order)
    {
        await context.Orders.AddAsync(order);
        return Result.Success();
    }

    public async Task<Result<Order>> GetByIdAsync(Guid id)
    {
        var order = await context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Game)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order is null
            ? Result<Order>.Failure(Error.NotFound("Orders.NotFound", $"Pedido com ID {id} nao encontrado."))
            : Result<Order>.Success(order);
    }

    public async Task<Result<IEnumerable<Order>>> GetByUserIdAsync(Guid userId)
    {
        var orders = await context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Game)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Result<IEnumerable<Order>>.Success(orders);
    }

    public async Task<Result> UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        return Result.Success();
    }
}
