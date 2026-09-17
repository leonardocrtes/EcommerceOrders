namespace EcommerceOrders.Application.DTOs.Responses;

public class ItemPedidoResponse
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
}