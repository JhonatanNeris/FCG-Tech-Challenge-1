using FCG.Domain.Common;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces;

public interface IGameRepository
{
    Task<Result<Game>> GetByIdAsync(Guid id);
    Task<Result<PagedResult<Game>>> GetAllAsync(PaginationParameters pagination);
    Task<Result> AddAsync(Game game);
    Task<Result> UpdateAsync(Game game);
}
