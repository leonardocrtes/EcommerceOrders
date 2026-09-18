using EcommerceOrders.Application.DTOs.Requests;
using EcommerceOrders.Application.Interfaces.Services;

namespace EcommerceOrders.Api.Endpoints;

public static class PedidoEndpoints
{
    public static IEndpointRouteBuilder MapearRotasPedidos(this IEndpointRouteBuilder rotas)
    {
        var grupo = rotas.MapGroup("/api/v1/pedidos")
                         .WithTags("Pedidos");

        grupo.MapPost("", async (CriarPedidoRequest request, IPedidoService pedidoService, CancellationToken ct) =>
        {
            var resultado = await pedidoService.CriarAsync(request, ct);
            return Results.Created($"/api/v1/pedidos/{resultado.Id}", resultado);
        })
        .WithName("CriarPedido")
        .WithSummary("Cria um novo pedido");

        grupo.MapGet("", async ([AsParameters] FiltroPedidoRequest filtro, IPedidoService pedidoService, CancellationToken ct) =>
        {
            var resultado = await pedidoService.ObterTodosAsync(filtro, ct);
            return Results.Ok(resultado);
        })
        .WithName("ListarPedidos")
        .WithSummary("Lista todos os pedidos com suporte a filtros");

        grupo.MapGet("/{id:guid}", async (Guid id, IPedidoService pedidoService, CancellationToken ct) =>
        {
            var resultado = await pedidoService.ObterPorIdAsync(id, ct);
            return resultado is null
                ? Results.NotFound(new { mensagem = $"Pedido com ID '{id}' não encontrado." })
                : Results.Ok(resultado);
        })
        .WithName("BuscarPedidoPorId")
        .WithSummary("Busca um pedido específico por ID");

        grupo.MapPut("/{id:guid}", async (Guid id, AlterarPedidoRequest request, IPedidoService pedidoService, CancellationToken ct) =>
        {
            var resultado = await pedidoService.AlterarAsync(id, request, ct);
            return Results.Ok(resultado);
        })
        .WithName("AlterarPedido")
        .WithSummary("Altera os itens de um pedido (apenas pedidos não processados)");

        grupo.MapPatch("/{id:guid}/cancelar", async (Guid id, IPedidoService pedidoService, CancellationToken ct) =>
        {
            var resultado = await pedidoService.CancelarAsync(id, ct);
            return Results.Ok(resultado);
        })
        .WithName("CancelarPedido")
        .WithSummary("Cancela um pedido (apenas pedidos iniciados ou processados)");

        grupo.MapPatch("/{id:guid}/processar", async (Guid id, IPedidoService pedidoService, CancellationToken ct) =>
        {
            var resultado = await pedidoService.ProcessarAsync(id, ct);
            return Results.Ok(resultado);
        })
        .WithName("ProcessarPedido")
        .WithSummary("Avança o status do pedido para Processado");

        grupo.MapPatch("/{id:guid}/enviar", async (Guid id, IPedidoService pedidoService, CancellationToken ct) =>
        {
            var resultado = await pedidoService.EnviarAsync(id, ct);
            return Results.Ok(resultado);
        })
        .WithName("EnviarPedido")
        .WithSummary("Marca o pedido como Enviado (apenas pedidos processados)");

        grupo.MapDelete("/{id:guid}", async (Guid id, IPedidoService pedidoService, CancellationToken ct) =>
        {
            await pedidoService.ExcluirAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("ExcluirPedido")
        .WithSummary("Exclui um pedido do sistema");

        return rotas;
    }
}