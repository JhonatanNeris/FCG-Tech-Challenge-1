using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;

namespace FCG.Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRep;
    private readonly IUserRepository _userRep;

    public GameService(IGameRepository gameRep, IUserRepository userRep)
    {
        _gameRep = gameRep;
        _userRep = userRep;
    }

    public async Task CreateAsync(CreateGameDto dto)
    {
        var game = new Game(dto.Title, dto.Description, dto.Price);
        await _gameRep.AddAsync(game);
    }

    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        var games = await _gameRep.GetAllAsync();
        return games.Select(MapToGameDto);
    }

    public async Task<GameDto> GetByIdAsync(Guid id)
    {
        var game = await _gameRep.GetByIdAsync(id) ?? throw new Exception("Jogo não encontrado.");
        return MapToGameDto(game);
    }

    public async Task AddToLibraryAsync(Guid userId, Guid gameId)
    {
        var user = await _userRep.GetByIdAsync(userId) ?? throw new Exception("Usuário não encontrado.");
        var game = await _gameRep.GetByIdAsync(gameId) ?? throw new Exception("Jogo não encontrado.");

        user.AddGameToLibrary(game);
        await _userRep.UpdateAsync(user);
    }

    public async Task<IEnumerable<GameDto>> GetLibraryAsync(Guid userId)
    {
        var user = await _userRep.GetByIdAsync(userId) ?? throw new Exception("Usuário não encontrado.");
        return user.LibraryItems.Select(ug => MapToGameDto(ug.Game));
    }

    private static GameDto MapToGameDto(Game g)
    {
        var now = DateTime.UtcNow;
        var activePromotion = g.Promotions
            .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
            .OrderByDescending(p => p.DiscountPercentage)
            .FirstOrDefault();

        if (activePromotion != null)
        {
            var discountedPrice = g.Price - (g.Price * activePromotion.DiscountPercentage / 100);
            return new GameDto(
                g.Id,
                g.Title,
                g.Description,
                discountedPrice,
                g.Price,
                activePromotion.DiscountPercentage,
                activePromotion.Name,
                true
            );
        }

        return new GameDto(g.Id, g.Title, g.Description, g.Price, g.Price);
    }
}
