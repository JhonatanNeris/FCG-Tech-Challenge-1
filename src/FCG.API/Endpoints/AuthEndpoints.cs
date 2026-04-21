using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FluentValidation;

namespace FCG.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Autenticação");

        group.MapPost("/register", async (RegisterUserDto dto, IAuthService service, IValidator<RegisterUserDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => x.ErrorMessage).ToArray()
                        ); return Results.ValidationProblem(errors);
            }

            await service.RegisterAsync(dto);
            return Results.Ok(new { message = "Usuário registrado com sucesso." });
        });

        group.MapPost("/login", async (LoginDto dto, IAuthService service) =>
        {
            var tokenDto = await service.LoginAsync(dto);
            return Results.Ok(tokenDto);
        });
    }
}
