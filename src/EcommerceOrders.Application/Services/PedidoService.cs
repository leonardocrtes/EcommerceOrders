using EcommerceOrders.Application.DTOs.Requests;
using EcommerceOrders.Application.DTOs.Responses;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Application.Interfaces.Services;
using EcommerceOrders.Domain.Constants;
using EcommerceOrders.Domain.Entities;
using EcommerceOrders.Domain.Enums;
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
        if (request.CompradorId == Guid.Empty)
            throw new RegraDeNegocioException(MensagensDeNegocio.CompradorObrigatorio);

        if (request.Itens == null || !request.Itens.Any())
            throw new RegraDeNegocioException(MensagensDeNegocio.PedidoSemProdutos);

        foreach (var item in request.Itens)
        {
            if (item.PrecoUnitario <= 0)
                throw new RegraDeNegocioException(MensagensDeNegocio.PrecoProdutoInvalido(item.NomeProduto));

            if (item.Quantidade <= 0)
                throw new RegraDeNegocioException(MensagensDeNegocio.QuantidadeProdutoInvalida(item.NomeProduto));
        }

        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            CompradorId = request.CompradorId,
            Status = StatusPedido.Iniciado,
            DataCriacao = DateTime.UtcNow
        };

        pedido.Itens = request.Itens.Select(item => new ItemPedido
        {
            Id = Guid.NewGuid(),
            PedidoId = pedido.Id,
            ProdutoId = item.ProdutoId,
            NomeProduto = item.NomeProduto,
            PrecoUnitario = item.PrecoUnitario,
            Quantidade = item.Quantidade
        }).ToList();

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
            throw new RegraDeNegocioException(MensagensDeNegocio.PedidoNaoEncontrado(id));

        if (pedido.Status != StatusPedido.Iniciado)
            throw new RegraDeNegocioException(MensagensDeNegocio.ApenasPedidosNaoProcessadosPodemSerAlterados);

        if (request.Itens == null || !request.Itens.Any())
            throw new RegraDeNegocioException(MensagensDeNegocio.PedidoSemProdutos);

        foreach (var item in request.Itens)
        {
            if (item.PrecoUnitario <= 0)
                throw new RegraDeNegocioException(MensagensDeNegocio.PrecoProdutoInvalido(item.NomeProduto));

            if (item.Quantidade <= 0)
                throw new RegraDeNegocioException(MensagensDeNegocio.QuantidadeProdutoInvalida(item.NomeProduto));
        }

        var novosItens = request.Itens.Select(item => new ItemPedido
        {
            Id = Guid.NewGuid(),
            PedidoId = pedido.Id,
            ProdutoId = item.ProdutoId,
            NomeProduto = item.NomeProduto,
            PrecoUnitario = item.PrecoUnitario,
            Quantidade = item.Quantidade
        }).ToList();

        pedido.Itens = novosItens;
        pedido.DataAtualizacao = DateTime.UtcNow;

        await _pedidoRepository.SubstituirItensAsync(pedido, novosItens, cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse> CancelarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        if (pedido is null)
            throw new RegraDeNegocioException(MensagensDeNegocio.PedidoNaoEncontrado(id));

        if (pedido.Status != StatusPedido.Iniciado && pedido.Status != StatusPedido.Processado)
            throw new RegraDeNegocioException(MensagensDeNegocio.ApenasPedidosIniciadosOuProcessadosPodemSerCancelados);

        pedido.Status = StatusPedido.Cancelado;
        pedido.DataAtualizacao = DateTime.UtcNow;

        await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse> ProcessarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        if (pedido is null)
            throw new RegraDeNegocioException(MensagensDeNegocio.PedidoNaoEncontrado(id));

        if (pedido.Status != StatusPedido.Iniciado)
            throw new RegraDeNegocioException(MensagensDeNegocio.StatusInvalidoParaProcessamento(pedido.Status.ToString()));

        pedido.Status = StatusPedido.Processado;
        pedido.DataAtualizacao = DateTime.UtcNow;

        await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse> EnviarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        if (pedido is null)
            throw new RegraDeNegocioException(MensagensDeNegocio.PedidoNaoEncontrado(id));

        if (pedido.Status != StatusPedido.Processado)
            throw new RegraDeNegocioException(MensagensDeNegocio.ApenasPedidosProcessadosPodemSerEnviados);

        pedido.Status = StatusPedido.Enviado;
        pedido.DataAtualizacao = DateTime.UtcNow;

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

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id, cancellationToken);
        if (pedido is null)
            throw new RegraDeNegocioException(MensagensDeNegocio.PedidoNaoEncontrado(id));

        await _pedidoRepository.RemoverAsync(pedido, cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);
    }
}