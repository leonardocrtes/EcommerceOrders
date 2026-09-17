using EcommerceOrders.Domain.Enums;
using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Domain.Entities;

public class Pedido
{
    public Guid Id { get; private set; }
    public Guid CompradorId { get; private set; }
    public StatusPedido Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    public Comprador? Comprador { get; private set; }

    private readonly List<ItemPedido> _itens = new();
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    public decimal ValorTotalPedido => _itens.Sum(item => item.ValorTotal);

    protected Pedido() { }

    public Pedido(Guid compradorId, IEnumerable<ItemPedido> itens)
    {
        if (compradorId == Guid.Empty)
            throw new RegraDeNegocioException("O comprador é obrigatório para criar um pedido.");

        var listaItens = itens?.ToList() ?? new List<ItemPedido>();
        if (!listaItens.Any())
            throw new RegraDeNegocioException("Todo pedido deve possuir pelo menos um produto.");

        Id = Guid.NewGuid();
        CompradorId = compradorId;
        Status = StatusPedido.Iniciado;
        DataCriacao = DateTime.UtcNow;
        _itens.AddRange(listaItens);
    }

    public bool PodeSerAlterado() => Status == StatusPedido.Iniciado;

    public bool PodeSerCancelado() => Status == StatusPedido.Iniciado || Status == StatusPedido.Processado;

    public bool PodeSerEnviado() => Status == StatusPedido.Processado;

    // Apenas pedidos não processados podem ser alterados
    public void AlterarItens(IEnumerable<ItemPedido> novosItens)
    {
        if (!PodeSerAlterado())
            throw new RegraDeNegocioException("Apenas pedidos não processados (status Iniciado) podem ser alterados.");

        var listaItens = novosItens?.ToList() ?? new List<ItemPedido>();
        if (!listaItens.Any())
            throw new RegraDeNegocioException("Todo pedido deve possuir pelo menos um produto.");

        _itens.Clear();
        _itens.AddRange(listaItens);
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Processar()
    {
        if (Status != StatusPedido.Iniciado)
            throw new RegraDeNegocioException($"Apenas pedidos com status 'Iniciado' podem ser processados. Status atual: '{Status}'.");

        Status = StatusPedido.Processado;
        DataAtualizacao = DateTime.UtcNow;
    }

    // Apenas pedidos processados podem ser enviados
    public void MarcarComoEnviado()
    {
        if (!PodeSerEnviado())
            throw new RegraDeNegocioException("Apenas pedidos com status 'Processado' podem ser enviados.");

        Status = StatusPedido.Enviado;
        DataAtualizacao = DateTime.UtcNow;
    }

    // Apenas pedidos iniciados ou processados podem ser cancelados
    public void Cancelar()
    {
        if (!PodeSerCancelado())
            throw new RegraDeNegocioException("Apenas pedidos iniciados ou processados podem ser cancelados.");

        Status = StatusPedido.Cancelado;
        DataAtualizacao = DateTime.UtcNow;
    }
}