using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using FCG.Domain.Interfaces;

namespace FCG.Application.Services;

public class OrderService : IOrderService
{
    private readonly IGameRepository _gameRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPromotionRepository _promotionRepository;


    public OrderService(IGameRepository gameRepository, IOrderRepository orderRepository, IUserRepository userRepository, IPromotionRepository promotionRepository)
    {
        _gameRepository = gameRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _promotionRepository = promotionRepository;
    }

    public async Task ApprovePaymentAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new Exception($"Pedido com ID {orderId} não encontrado.");
        }
        if (order.Status != OrderStatus.Pending)
        {
            throw new Exception($"Pedido com ID {orderId} não está em status Pendente.");
        }

        var user = await _userRepository.GetByIdAsync(order.UserId);
        if (user == null) throw new Exception("Usuário não encontrado.");

        order.MarkAsPaid();

        foreach (var item in order.Items)
        {
            user.AddGameToLibrary(item.Game);
        }

        await _orderRepository.UpdateAsync(order);
        await _userRepository.UpdateAsync(user);

    }

    public async Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto dto)
    {
        var order = new Order(userId);

        foreach (var gameId in dto.GameIds)
        {
            {
                var game = await _gameRepository.GetByIdAsync(gameId);

                if (game == null)
                {
                    throw new Exception($"Jogo com ID {gameId} não encontrado.");
                }

                var activePromotions = await _promotionRepository.GetActivePromotionsByGameIdAsync(gameId);
                var bestPromotion = activePromotions.OrderByDescending(p => p.DiscountPercentage).FirstOrDefault();

                decimal priceAtPurchase = game.Price;

                if (bestPromotion != null)
                {
                    priceAtPurchase -= bestPromotion.CalculateDiscountAmount(game.Price);
                }

                order.AddItem(gameId, priceAtPurchase);
            }
        }

        await _orderRepository.AddAsync(order);
        return MapToDto(order);

    }

    public async Task<OrderDto> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new Exception($"Pedido com ID {orderId} não encontrado.");
        }
        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        if (orders == null || !orders.Any())
        {
            throw new Exception($"Nenhum pedido encontrado para o usuário com ID {userId}.");
        }
        return orders.Select(MapToDto);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.TotalAmount,
            order.Status.ToString(),
            order.Items.Select(i => new OrderItemDto(
                i.GameId,
                i.Game?.Title ?? "Jogo Desconhecido",
                i.PriceAtPurchase
            )).ToList()
        );
    }
}
