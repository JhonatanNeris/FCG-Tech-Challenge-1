using FCG.Domain.Common;
using FCG.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FCG.Infrastructure.Data;

public sealed class UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    public async Task<Result> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao salvar alteracoes no banco de dados.");
            return Result.Failure(Errors.UnitOfWork.CommitFailed);
        }
    }
}
