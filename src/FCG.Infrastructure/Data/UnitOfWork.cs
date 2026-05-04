using FCG.Domain.Common;
using FCG.Domain.Interfaces;

namespace FCG.Infrastructure.Data;

public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
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
            return Result.Failure(Error.Failure("UnitOfWork.CommitFailed", ex.Message));
        }
    }
}
