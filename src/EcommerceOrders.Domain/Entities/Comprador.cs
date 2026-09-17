using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Domain.Entities;
public class Comprador
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    protected Comprador() { }

    public Comprador(Guid id, string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new RegraDeNegocioException("O nome do comprador é obrigatório.");

        if (string.IsNullOrWhiteSpace(email))
            throw new RegraDeNegocioException("O e-mail do comprador é obrigatório.");

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Nome = nome;
        Email = email;
    }
}