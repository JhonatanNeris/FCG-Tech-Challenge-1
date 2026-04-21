using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FCG.API.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        // CRUD de Games (Catálogo)
        var group = app.MapGroup("/api/games").WithTags("Jogos (Catálogo)").RequireAuthorization();

        group.MapPost("/", [Authorize(Roles = "Admin")] async (CreateGameDto dto, IGameService service) =>
        {
            await service.CreateAsync(dto);
            return Results.Ok(new { message = "Jogo criado com sucesso no catálogo." });
        });

        group.MapGet("/", async (IGameService service) =>
        {
            var games = await service.GetAllAsync();
            return Results.Ok(games);
        }).AllowAnonymous(); // Listagem pública de catálogo

        group.MapGet("/{id:guid}", async (Guid id, IGameService service) =>
        {
            var game = await service.GetByIdAsync(id);
            return game is not null ? Results.Ok(game) : Results.NotFound(new { message = "Jogo não encontrado." });
        }).AllowAnonymous(); // Detalhes públicos de catálogo

        // Gerenciamento da Biblioteca do Usuário autenticado
        var library = app.MapGroup("/api/library").WithTags("Biblioteca do Usuário").RequireAuthorization();

        library.MapGet("/", async (IGameService service, ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var games = await service.GetLibraryAsync(userId);
            return Results.Ok(games);
        });
    }
}

