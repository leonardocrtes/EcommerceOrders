using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Domain.Entities;
public class Produto
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }

    protected Produto() { }

    public Produto(Guid id, string nome, decimal preco)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new RegraDeNegocioException("O nome do produto é obrigatório.");

        if (preco <= 0)
            throw new RegraDeNegocioException("O preço do produto deve ser maior que zero.");

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Nome = nome;
        Preco = preco;
    }
}