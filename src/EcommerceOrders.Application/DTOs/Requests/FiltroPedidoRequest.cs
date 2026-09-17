using EcommerceOrders.Domain.Enums;

namespace EcommerceOrders.Application.DTOs.Requests;

public class FiltroPedidoRequest
{
    public StatusPedido? Status { get; set; }
    public Guid? CompradorId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}