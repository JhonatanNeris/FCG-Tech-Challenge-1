using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;

    public PromotionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Promotion?> GetByIdAsync(Guid id)
    {
        return await _context.Promotions
            .Include(p => p.Game)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Promotion>> GetAllAsync()
    {
        return await _context.Promotions
            .Include(p => p.Game)
            .ToListAsync();
    }

    public async Task<IEnumerable<Promotion>> GetActivePromotionsByGameIdAsync(Guid gameId)
    {
        var now = DateTime.UtcNow;
        return await _context.Promotions
            .Where(p => p.GameId == gameId && p.IsActive && p.StartDate <= now && p.EndDate >= now)
            .ToListAsync();
    }

    public async Task AddAsync(Promotion promotion)
    {
        await _context.Promotions.AddAsync(promotion);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Promotion promotion)
    {
        _context.Promotions.Update(promotion);
        await _context.SaveChangesAsync();
    }
}