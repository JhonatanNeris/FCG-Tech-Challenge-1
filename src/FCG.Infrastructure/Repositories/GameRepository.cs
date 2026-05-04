using FCG.Domain.Common;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Infrastructure.Data;
using FCG.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Repositories;

public class GameRepository(AppDbContext context) : IGameRepository
{
    public async Task<Result> AddAsync(Game game)
    {
        await context.Games.AddAsync(game);
        return Result.Success();
    }

    public async Task<Result<PagedResult<Game>>> GetAllAsync(PaginationParameters pagination)
    {
        var query = context.Games
            .Include(g => g.Promotions)
            .Where(g => g.IsActive)
            .OrderBy(g => g.Title);

        return Result<PagedResult<Game>>.Success(await query.ToPagedResultAsync(pagination));
    }

    public async Task<Result<Game>> GetByIdAsync(Guid id)
    {
        var game = await context.Games
            .Include(g => g.Promotions)
            .FirstOrDefaultAsync(g => g.Id == id);

        return game is null
            ? Result<Game>.Failure(Error.NotFound("Games.NotFound", "Jogo nao encontrado."))
            : Result<Game>.Success(game);
    }

    public async Task<Result> UpdateAsync(Game game)
    {
        context.Games.Update(game);
        return Result.Success();
    }
}
