using FCG.Domain.Common;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<Result<User>> GetByEmailAsync(string email);
}
