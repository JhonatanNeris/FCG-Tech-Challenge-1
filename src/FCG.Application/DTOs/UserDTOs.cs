namespace FCG.Application.DTOs;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Role);

public record CreateUserDto(string Name, string Email, string Password, string Role);

public record UpdateUserRoleDto(string Role);
