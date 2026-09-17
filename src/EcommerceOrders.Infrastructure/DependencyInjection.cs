using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Infrastructure.Data;
using EcommerceOrders.Infrastructure.Repositories;

namespace EcommerceOrders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("A ConnectionString 'DefaultConnection' não foi configurada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IPedidoRepository, PedidoRepository>();

        return services;
    }
}