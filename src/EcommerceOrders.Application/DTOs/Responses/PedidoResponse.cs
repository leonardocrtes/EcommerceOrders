using EcommerceOrders.Domain.Enums;

namespace EcommerceOrders.Application.DTOs.Responses;

public class PedidoResponse
{
    public Guid Id { get; set; }
    public Guid CompradorId { get; set; }
    public StatusPedido Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public decimal ValorTotalPedido { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public List<ItemPedidoResponse> Itens { get; set; } = new();
}