using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces;

public interface IPromotionRepository
{
    Task<Promotion?> GetByIdAsync(Guid id);
    Task<IEnumerable<Promotion>> GetAllAsync();
    Task<IEnumerable<Promotion>> GetActivePromotionsByGameIdAsync(Guid gameId);
    Task AddAsync(Promotion promotion);
    Task UpdateAsync(Promotion promotion);
}