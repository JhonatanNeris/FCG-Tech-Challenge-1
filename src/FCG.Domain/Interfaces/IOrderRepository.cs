using FCG.Domain.Common;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Result<Order>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<Order>>> GetByUserIdAsync(Guid userId);
    Task<Result> AddAsync(Order order);
    Task<Result> UpdateAsync(Order order);
}
