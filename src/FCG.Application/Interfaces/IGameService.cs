using FCG.Application.DTOs;

namespace FCG.Application.Interfaces;

public interface IGameService
{
    Task<IEnumerable<GameDto>> GetAllAsync();
    Task<GameDto> GetByIdAsync(Guid id);
    Task CreateAsync(CreateGameDto dto);
    Task AddToLibraryAsync(Guid userId, Guid gameId);
    Task<IEnumerable<GameDto>> GetLibraryAsync(Guid userId);
}
