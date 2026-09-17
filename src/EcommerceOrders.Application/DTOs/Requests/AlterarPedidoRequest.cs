namespace EcommerceOrders.Application.DTOs.Requests;

public class AlterarPedidoRequest
{
    public List<ItemPedidoRequest> Itens { get; set; } = new();
}