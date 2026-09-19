using EcommerceOrders.Api.Endpoints;
using EcommerceOrders.Api.Middlewares;
using EcommerceOrders.Application.Interfaces.Services;
using EcommerceOrders.Application.Services;
using EcommerceOrders.Infrastructure;
using EcommerceOrders.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ecommerce Orders API",
        Version = "v1",
        Description = "API RESTful para gerenciamento de pedidos de e-commerce"
    });
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("SQL Server (Docker)");

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratamentoGlobalErrosHandler>();
builder.Services.AdicionarInfraestrutura(builder.Configuration);
builder.Services.AddScoped<IPedidoService, PedidoService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });

    app.UseSwagger(options =>
    {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
        options.SerializeAsV2 = true;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce Orders API v1");
    });

    app.MapScalarApiReference();
}

app.UseCors();
app.UseHttpsRedirection();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var resposta = new
        {
            status = report.Status.ToString(),
            tempoResposta = $"{report.TotalDuration.TotalMilliseconds:0.00} ms",
            dataHora = DateTime.UtcNow,
            dependencias = report.Entries.Select(e => new
            {
                componente = e.Key,
                status = e.Value.Status.ToString(),
                duracao = $"{e.Value.Duration.TotalMilliseconds:0.00} ms",
                descricao = e.Value.Description ?? "Conexão com o banco de dados ativa e operacional"
            })
        };
        await context.Response.WriteAsJsonAsync(resposta);
    }
});

app.MapearRotasPedidos();

app.Run();