using EcommerceOrders.Application.DTOs.Requests;
using EcommerceOrders.Domain.Entities;

namespace EcommerceOrders.Application.Interfaces.Repositories;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pedido>> ObterTodosAsync(FiltroPedidoRequest? filtro = null, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Pedido pedido, CancellationToken cancellationToken = default);
    Task SubstituirItensAsync(Pedido pedido, IEnumerable<ItemPedido> novosItens, CancellationToken cancellationToken = default);
    Task RemoverAsync(Pedido pedido, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}