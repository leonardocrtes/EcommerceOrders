using System;

namespace EcommerceOrders.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
}