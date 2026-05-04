using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Application.Settings;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using FCG.Domain.Interfaces;
using Isopoh.Cryptography.Argon2;

namespace FCG.Application.Services;

public class AuthService : IAuthService
{
    private const int Argon2MemoryCost = 65536;
    private const int Argon2Iterations = 3;
    private const int Argon2Threads = 4;
    private const int Argon2HashLength = 32;

    private readonly IUserRepository _userRep;
    private readonly ITokenService _tokenService;
    private readonly string _secretKey;

    public AuthService(IUserRepository userRep, ITokenService tokenService, AuthSettings authSettings)
    {
        _userRep = userRep;
        _tokenService = tokenService;
        _secretKey = authSettings.SecretKey;
    }

    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRep.GetByEmailAsync(dto.Email) ?? throw new Exception("Credenciais inválidas.");
        if (!VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Credenciais inválidas.");

        return new TokenDto(_tokenService.GenerateToken(user));
    }

    public async Task RegisterAsync(RegisterUserDto dto)
    {
        if (await _userRep.GetByEmailAsync(dto.Email) != null)
            throw new Exception("Email já está em uso.");

        var passHash = HashPassword(dto.Password);

        // Opcional: atribuir role. Por padrao daremos Admin para emails contendo 'admin'.
        var role = dto.Email.ToLower().Contains("admin") ? Role.Admin : Role.User;
        var user = new User(dto.Name, dto.Email, passHash, role);
        await _userRep.AddAsync(user);
    }

    private string HashPassword(string password)
    {
        return Argon2.Hash(
            ConcatPasswordWithSecret(password),
            timeCost: Argon2Iterations,
            memoryCost: Argon2MemoryCost,
            parallelism: Argon2Threads,
            type: Argon2Type.HybridAddressing,
            hashLength: Argon2HashLength);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        return Argon2.Verify(passwordHash, ConcatPasswordWithSecret(password));
    }

    private string ConcatPasswordWithSecret(string password)
    {
        return $"{password}{_secretKey}";
    }
}
