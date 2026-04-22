using FCG.Application.DTOs;

namespace FCG.Application.Interfaces;

public interface IAuthService
{
    // Define os métodos que o serviço de autenticação deve implementar
    Task<TokenDto> LoginAsync(LoginDto dto);
    Task RegisterAsync(RegisterUserDto dto);
}
