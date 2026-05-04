using FCG.Domain.Common;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces;

public interface IUserRepository
{
    Task<Result<User>> GetByIdAsync(Guid id);
    Task<Result<User>> GetByEmailAsync(string email);
    Task<Result> AddAsync(User user);
    Task<Result> UpdateAsync(User user);
}
