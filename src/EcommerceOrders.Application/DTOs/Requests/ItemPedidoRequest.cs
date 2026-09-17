using System;

namespace EcommerceOrders.Application.DTOs.Requests;

public class ItemPedidoRequest
{
    public Guid ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
}