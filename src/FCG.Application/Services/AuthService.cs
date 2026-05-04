using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Application.Security;
using FCG.Application.Settings;
using FCG.Domain.Common;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using FCG.Domain.Interfaces;

namespace FCG.Application.Services;

public class AuthService(
    IUserRepository userRep,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    AuthSettings authSettings) : IAuthService
{
    private readonly string _secretKey = authSettings.SecretKey;

    public async Task<Result<TokenDto>> LoginAsync(LoginDto dto)
    {
        var userResult = await userRep.GetByEmailAsync(dto.Email);
        if (userResult.IsFailure || !PasswordHasher.VerifyPassword(dto.Password, userResult.Value.PasswordHash, _secretKey))
        {
            return Result<TokenDto>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Credenciais invalidas."));
        }

        return Result<TokenDto>.Success(new TokenDto(tokenService.GenerateToken(userResult.Value)));
    }

    public async Task<Result> RegisterAsync(RegisterUserDto dto)
    {
        var existingUser = await userRep.GetByEmailAsync(dto.Email);
        if (existingUser.IsSuccess)
        {
            return Result.Failure(Error.Validation("Auth.EmailAlreadyInUse", "Email ja esta em uso."));
        }

        var passHash = PasswordHasher.HashPassword(dto.Password, _secretKey);
        var user = new User(dto.Name, dto.Email, passHash, Role.User);

        var addResult = await userRep.AddAsync(user);
        return addResult.IsFailure ? addResult : await unitOfWork.CommitAsync();
    }

}
