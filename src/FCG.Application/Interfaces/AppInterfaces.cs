using FCG.Application.DTOs;
using FCG.Domain.Entities;

namespace FCG.Application.Interfaces;

public interface IAuthService
{
    // Define os métodos que o serviço de autenticação deve implementar
    Task<TokenDto> LoginAsync(LoginDto dto);
    Task RegisterAsync(RegisterUserDto dto);
}

public interface IGameService
{
    Task<IEnumerable<GameDto>> GetAllAsync();
    Task<GameDto> GetByIdAsync(Guid id);
    Task CreateAsync(CreateGameDto dto);
    Task AddToLibraryAsync(Guid userId, Guid gameId);
    Task<IEnumerable<GameDto>> GetLibraryAsync(Guid userId);
}

public interface ITokenService
{
    string GenerateToken(User user);
}

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto dto);
    Task<OrderDto> GetOrderByIdAsync(Guid orderId);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId);
    Task ApprovePaymentAsync(Guid orderId);
}
