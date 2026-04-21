using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using System.Security.Claims;

namespace FCG.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Pedidos").RequireAuthorization();

        group.MapPost("/", async (CreateOrderDto dto, IOrderService service, ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var order = await service.CreateOrderAsync(userId, dto);
            return Results.Created($"/api/orders/{order.Id}", order);
        });

        group.MapGet("/{id:guid}", async (Guid id, IOrderService service) =>
        {
            var order = await service.GetOrderByIdAsync(id);
            return Results.Ok(order);
        });

        group.MapGet("/", async (IOrderService service, ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var orders = await service.GetUserOrdersAsync(userId);
            return Results.Ok(orders);
        });

        group.MapPost("/{id:guid}/pay", async (Guid id, IOrderService service) =>
        {
            await service.ApprovePaymentAsync(id);
            return Results.Ok(new { message = "Pagamento aprovado e jogos adicionados à biblioteca." });
        });
    }
}