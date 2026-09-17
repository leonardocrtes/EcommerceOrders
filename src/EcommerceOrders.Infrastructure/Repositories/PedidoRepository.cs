using Microsoft.EntityFrameworkCore;
using EcommerceOrders.Application.DTOs.Requests;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Domain.Entities;
using EcommerceOrders.Infrastructure.Data;

namespace EcommerceOrders.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .Include(p => p.Comprador)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Pedido>> ObterTodosAsync(FiltroPedidoRequest? filtro = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .Include(p => p.Comprador)
            .AsQueryable();

        if (filtro is not null)
        {
            if (filtro.Status.HasValue)
                query = query.Where(p => p.Status == filtro.Status.Value);

            if (filtro.CompradorId.HasValue && filtro.CompradorId.Value != Guid.Empty)
                query = query.Where(p => p.CompradorId == filtro.CompradorId.Value);

            if (filtro.DataInicio.HasValue)
                query = query.Where(p => p.DataCriacao >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(p => p.DataCriacao <= filtro.DataFim.Value);
        }

        return await query.OrderByDescending(p => p.DataCriacao).ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        await _context.Pedidos.AddAsync(pedido, cancellationToken);
    }

    public Task AtualizarAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        _context.Pedidos.Update(pedido);
        return Task.CompletedTask;
    }

    public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}