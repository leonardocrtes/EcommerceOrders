
using EcommerceOrders.Application.DTOs.Requests;
using EcommerceOrders.Application.DTOs.Responses;

namespace EcommerceOrders.Application.Interfaces.Services;

public interface IPedidoService
{
    Task<PedidoResponse> CriarAsync(CriarPedidoRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<PedidoResponse>> ObterTodosAsync(FiltroPedidoRequest? filtro = null, CancellationToken cancellationToken = default);
    Task<PedidoResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PedidoResponse> AlterarAsync(Guid id, AlterarPedidoRequest request, CancellationToken cancellationToken = default);
    Task<PedidoResponse> CancelarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PedidoResponse> ProcessarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PedidoResponse> EnviarAsync(Guid id, CancellationToken cancellationToken = default);
}