using FCG.Domain.Common;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Infrastructure.Data;
using FCG.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Repositories;

public class PromotionRepository(AppDbContext context) : IPromotionRepository
{
    public async Task<Result<Promotion>> GetByIdAsync(Guid id)
    {
        var promotion = await context.Promotions
            .Include(p => p.Game)
            .FirstOrDefaultAsync(p => p.Id == id);

        return promotion is null
            ? Result<Promotion>.Failure(Error.NotFound("Promotions.NotFound", "Promocao nao encontrada."))
            : Result<Promotion>.Success(promotion);
    }

    public async Task<Result<PagedResult<Promotion>>> GetAllAsync(PaginationParameters pagination)
    {
        var query = context.Promotions
            .Include(p => p.Game)
            .OrderBy(p => p.Name);

        return Result<PagedResult<Promotion>>.Success(await query.ToPagedResultAsync(pagination));
    }

    public async Task<Result<PagedResult<Promotion>>> GetAllActiveAsync(PaginationParameters pagination)
    {
        var now = DateTime.UtcNow;
        var query = context.Promotions
            .Include(p => p.Game)
            .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
            .OrderBy(p => p.Name);

        return Result<PagedResult<Promotion>>.Success(await query.ToPagedResultAsync(pagination));
    }

    public async Task<Result<IEnumerable<Promotion>>> GetActivePromotionsByGameIdAsync(Guid gameId)
    {
        var now = DateTime.UtcNow;
        var promotions = await context.Promotions
            .Where(p => p.GameId == gameId && p.IsActive && p.StartDate <= now && p.EndDate >= now)
            .ToListAsync();

        return Result<IEnumerable<Promotion>>.Success(promotions);
    }

    public async Task<Result> AddAsync(Promotion promotion)
    {
        await context.Promotions.AddAsync(promotion);
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(Promotion promotion)
    {
        context.Promotions.Update(promotion);
        return Result.Success();
    }
}
