using FCG.Domain.Common;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Infrastructure.Data;
using FCG.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Repositories;

public class UserRepository(AppDbContext context)
    : Repository<User>(context, Errors.Users.NotFound), IUserRepository
{
    protected override IQueryable<User> Query => Context.Users
        .Include(user => user.LibraryItems)
        .ThenInclude(libraryItem => libraryItem.Game);

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        var user = await Context.Users.FirstOrDefaultAsync(user => user.Email == email);

        return user is null
            ? Result<User>.Failure(Errors.Users.NotFoundByEmail)
            : Result<User>.Success(user);
    }

    public async Task<Result<PagedResult<User>>> GetAllAsync(PaginationParameters pagination)
    {
        var query = Context.Users.OrderBy(user => user.Name).ThenBy(user => user.Email);

        return Result<PagedResult<User>>.Success(await query.ToPagedResultAsync(pagination));
    }
}
