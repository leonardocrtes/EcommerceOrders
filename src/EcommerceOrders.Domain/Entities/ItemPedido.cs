using System;

namespace EcommerceOrders.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PedidoId { get; set; }
    public Guid ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal => PrecoUnitario * Quantidade;
}