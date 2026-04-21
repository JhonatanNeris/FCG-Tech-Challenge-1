using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace FCG.API.Endpoints;

public static class PromotionEndpoints
{
    public static void MapPromotionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/promotions").WithTags("Promoções").RequireAuthorization();

        group.MapPost("/", [Authorize(Roles = "Admin")] async (CreatePromotionDto dto, IPromotionService service, IValidator<CreatePromotionDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.ToDictionary(e => e.PropertyName, e => new[] { e.ErrorMessage });
                return Results.ValidationProblem(errors);
            }

            await service.CreateAsync(dto);
            return Results.Ok(new { message = "Promoção criada com sucesso." });
        });

        group.MapGet("/", async (IPromotionService service) =>
        {
            var promotions = await service.GetAllActiveAsync();
            return Results.Ok(promotions);
        }).AllowAnonymous();

        group.MapDelete("/{id:guid}", [Authorize(Roles = "Admin")] async (Guid id, IPromotionService service) =>
        {
            await service.DeactivateAsync(id);
            return Results.Ok(new { message = "Promoção desativada com sucesso." });
        });
    }
}