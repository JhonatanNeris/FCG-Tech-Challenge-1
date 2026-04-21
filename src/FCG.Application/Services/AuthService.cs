using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using FCG.Domain.Interfaces;

namespace FCG.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRep;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRep, ITokenService tokenService)
    {
        _userRep = userRep;
        _tokenService = tokenService;
    }

    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRep.GetByEmailAsync(dto.Email) ?? throw new Exception("Credenciais inválidas.");
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Credenciais inválidas.");

        return new TokenDto(_tokenService.GenerateToken(user));
    }

    public async Task RegisterAsync(RegisterUserDto dto)
    {
        if (await _userRep.GetByEmailAsync(dto.Email) != null)
            throw new Exception("Email já está em uso.");

        var passHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Opcional: atribuir role. Por padrao daremos Admin para emails contendo 'admin'.
        var role = dto.Email.ToLower().Contains("admin") ? Role.Admin : Role.User;
        var user = new User(dto.Name, dto.Email, passHash, role);
        await _userRep.AddAsync(user);
    }
}
