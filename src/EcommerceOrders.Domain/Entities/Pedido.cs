using EcommerceOrders.Domain.Enums;

namespace EcommerceOrders.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CompradorId { get; set; }
    public StatusPedido Status { get; set; } = StatusPedido.Iniciado;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
    public decimal ValorTotalPedido => Itens.Sum(i => i.ValorTotal);
}