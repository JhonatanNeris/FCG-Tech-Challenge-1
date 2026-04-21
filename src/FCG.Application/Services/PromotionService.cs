using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;

namespace FCG.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IGameRepository _gameRepository;

    public PromotionService(IPromotionRepository promotionRepository, IGameRepository gameRepository)
    {
        _promotionRepository = promotionRepository;
        _gameRepository = gameRepository;
    }

    public async Task<IEnumerable<PromotionDto>> GetAllActiveAsync()
    {
        var promotions = await _promotionRepository.GetAllAsync();
        var now = DateTime.UtcNow;

        return promotions
            .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
            .Select(p => new PromotionDto(
                p.Id,
                p.Name,
                p.GameId,
                p.Game.Title,
                p.DiscountPercentage,
                p.StartDate,
                p.EndDate,
                p.IsActive
            ));
    }

    public async Task CreateAsync(CreatePromotionDto dto)
    {
        var game = await _gameRepository.GetByIdAsync(dto.GameId);
        if (game == null)
            throw new Exception("Jogo não encontrado.");

        var promotion = new Promotion(
            dto.Name,
            dto.GameId,
            dto.DiscountPercentage,
            dto.StartDate,
            dto.EndDate
        );

        await _promotionRepository.AddAsync(promotion);
    }

    public async Task DeactivateAsync(Guid id)
    {
        var promotion = await _promotionRepository.GetByIdAsync(id);
        if (promotion == null)
            throw new Exception("Promoção não encontrada.");

        promotion.Deactivate();
        await _promotionRepository.UpdateAsync(promotion);
    }
}
