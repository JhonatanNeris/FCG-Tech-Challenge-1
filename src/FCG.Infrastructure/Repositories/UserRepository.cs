using FCG.Domain.Common;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<Result> AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        return Result.Success();
    }

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        return user is null
            ? Result<User>.Failure(Error.NotFound("Users.NotFoundByEmail", "Usuario nao encontrado."))
            : Result<User>.Success(user);
    }

    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        var user = await context.Users
            .Include(u => u.LibraryItems)
            .ThenInclude(ug => ug.Game)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user is null
            ? Result<User>.Failure(Error.NotFound("Users.NotFound", "Usuario nao encontrado."))
            : Result<User>.Success(user);
    }

    public async Task<Result> UpdateAsync(User user)
    {
        context.Users.Update(user);
        return Result.Success();
    }
}
