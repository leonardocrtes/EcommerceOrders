using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public decimal PrecoUnitario { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorTotal => PrecoUnitario * Quantidade;

    protected ItemPedido() { }

    public ItemPedido(Guid produtoId, string nomeProduto, decimal precoUnitario, int quantidade)
    {
        if (produtoId == Guid.Empty)
            throw new RegraDeNegocioException("O produto do item é obrigatório.");

        if (precoUnitario <= 0)
            throw new RegraDeNegocioException("O preço unitário do item deve ser maior que zero.");

        if (quantidade <= 0)
            throw new RegraDeNegocioException("A quantidade do item deve ser de pelo menos 1 unidade.");

        Id = Guid.NewGuid();
        ProdutoId = produtoId;
        NomeProduto = nomeProduto;
        PrecoUnitario = precoUnitario;
        Quantidade = quantidade;
    }
}