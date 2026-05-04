using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Application.Settings;
using FCG.Domain.Common;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;

namespace FCG.Application.Services;

public class GameService(
    IGameRepository gameRep,
    IUserRepository userRep,
    IUnitOfWork unitOfWork,
    PaginationSettings paginationSettings) : IGameService
{
    private readonly int _pageSize = paginationSettings.PageSize;

    public async Task<Result<GameDto>> CreateAsync(CreateGameDto dto)
    {
        var game = new Game(dto.Title, dto.Description, dto.Price);
        var addResult = await gameRep.AddAsync(game);

        if (addResult.IsFailure)
        {
            return Result<GameDto>.Failure(addResult.Error!);
        }

        var commitResult = await unitOfWork.CommitAsync();
        return commitResult.IsFailure
            ? Result<GameDto>.Failure(commitResult.Error!)
            : Result<GameDto>.Success(MapToGameDto(game));
    }

    public async Task<Result<PagedResult<GameDto>>> GetAllAsync(int page)
    {
        if (page < 1)
        {
            return Result<PagedResult<GameDto>>.Failure(Error.InvalidRequest("Pagination.InvalidPage", "Pagina deve ser maior ou igual a 1."));
        }

        var gamesResult = await gameRep.GetAllAsync(new PaginationParameters(page, _pageSize));
        if (gamesResult.IsFailure)
        {
            return Result<PagedResult<GameDto>>.Failure(gamesResult.Error!);
        }

        var games = gamesResult.Value;
        return Result<PagedResult<GameDto>>.Success(new PagedResult<GameDto>(
            games.Items.Select(MapToGameDto).ToArray(),
            games.Page,
            games.PageSize,
            games.TotalCount));
    }

    public async Task<Result<GameDto>> GetByIdAsync(Guid id)
    {
        var gameResult = await gameRep.GetByIdAsync(id);
        return gameResult.IsFailure
            ? Result<GameDto>.Failure(gameResult.Error!)
            : Result<GameDto>.Success(MapToGameDto(gameResult.Value));
    }

    public async Task<Result> AddToLibraryAsync(Guid userId, Guid gameId)
    {
        var userResult = await userRep.GetByIdAsync(userId);
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var gameResult = await gameRep.GetByIdAsync(gameId);
        if (gameResult.IsFailure)
        {
            return Result.Failure(gameResult.Error!);
        }

        userResult.Value.AddGameToLibrary(gameResult.Value);
        var updateResult = await userRep.UpdateAsync(userResult.Value);
        return updateResult.IsFailure ? updateResult : await unitOfWork.CommitAsync();
    }

    public async Task<Result<IEnumerable<GameDto>>> GetLibraryAsync(Guid userId)
    {
        var userResult = await userRep.GetByIdAsync(userId);
        return userResult.IsFailure
            ? Result<IEnumerable<GameDto>>.Failure(userResult.Error!)
            : Result<IEnumerable<GameDto>>.Success(userResult.Value.LibraryItems.Select(ug => MapToGameDto(ug.Game)));
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
