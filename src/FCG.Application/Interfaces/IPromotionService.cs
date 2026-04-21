using FCG.Application.DTOs;

namespace FCG.Application.Interfaces;

public interface IPromotionService
{
    Task<IEnumerable<PromotionDto>> GetAllActiveAsync();
    Task CreateAsync(CreatePromotionDto dto);
    Task DeactivateAsync(Guid id);
}
