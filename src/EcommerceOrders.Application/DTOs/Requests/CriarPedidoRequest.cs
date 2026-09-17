using System;
using System.Collections.Generic;

namespace EcommerceOrders.Application.DTOs.Requests;

public class CriarPedidoRequest
{
    public Guid CompradorId { get; set; }
    public List<ItemPedidoRequest> Itens { get; set; } = new();
}