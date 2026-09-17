using EcommerceOrders.Application.DTOs.Requests;
using EcommerceOrders.Application.DTOs.Responses;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Application.Interfaces.Services;
using EcommerceOrders.Domain.Entities;
using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;

    public PedidoService(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task<PedidoResponse> CriarAsync(CriarPedidoRequest request, CancellationToken cancellationToken = default)
    {
        var itens = request.Itens.Select(item =>
            new ItemPedido(item.ProdutoId, item.NomeProduto, item.PrecoUnitario, item.Quantidade)
        );

        var pedido = new Pedido(request.CompradorId, itens);

        await _pedidoRepository.AdicionarAsync(pedido, cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return MapearParaResponse(pedido);
    }

    public async Task<IEnumerable<PedidoResponse>> ObterTodosAsync(FiltroPedidoRequest? filtro = null, CancellationToken cancellationToken = default)
    {
        var pedidos = await _pedidoRepository.ObterTodosAsync(filtro, cancellationToken);
        return pedidos.Select(MapearParaResponse);
    }

    public async Task<PedidoResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        return pedido is null ? null : MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse> AlterarAsync(Guid id, AlterarPedidoRequest request, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        if (pedido is null)
            throw new RegraDeNegocioException($"Pedido com ID '{id}' não foi encontrado.");

        var novosItens = request.Itens.Select(item =>
            new ItemPedido(item.ProdutoId, item.NomeProduto, item.PrecoUnitario, item.Quantidade)
        );

        pedido.AlterarItens(novosItens);

        await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse> CancelarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        if (pedido is null)
            throw new RegraDeNegocioException($"Pedido com ID '{id}' não foi encontrado.");

        pedido.Cancelar();

        await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse> ProcessarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        if (pedido is null)
            throw new RegraDeNegocioException($"Pedido com ID '{id}' não foi encontrado.");

        pedido.Processar();

        await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse> EnviarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        if (pedido is null)
            throw new RegraDeNegocioException($"Pedido com ID '{id}' não foi encontrado.");

        pedido.MarcarComoEnviado();

        await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return MapearParaResponse(pedido);
    }

    private static PedidoResponse MapearParaResponse(Pedido pedido)
    {
        return new PedidoResponse
        {
            Id = pedido.Id,
            CompradorId = pedido.CompradorId,
            Status = pedido.Status,
            ValorTotalPedido = pedido.ValorTotalPedido,
            DataCriacao = pedido.DataCriacao,
            DataAtualizacao = pedido.DataAtualizacao,
            Itens = pedido.Itens.Select(item => new ItemPedidoResponse
            {
                Id = item.Id,
                ProdutoId = item.ProdutoId,
                NomeProduto = item.NomeProduto,
                PrecoUnitario = item.PrecoUnitario,
                Quantidade = item.Quantidade,
                ValorTotal = item.ValorTotal
            }).ToList()
        };
    }
}