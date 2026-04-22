using FCG.Application.DTOs;

namespace FCG.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto dto);
    Task<OrderDto> GetOrderByIdAsync(Guid orderId);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId);
    Task ApprovePaymentAsync(Guid orderId);
}
