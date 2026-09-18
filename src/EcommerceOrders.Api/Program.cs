using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using EcommerceOrders.Api.Endpoints;
using EcommerceOrders.Api.Middlewares;
using EcommerceOrders.Application.Interfaces.Services;
using EcommerceOrders.Application.Services;
using EcommerceOrders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ecommerce Orders API",
        Version = "v1",
        Description = "API RESTful para gerenciamento de pedidos de e-commerce"
    });
});

builder.Services.AddHealthChecks();
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
        options.SerializeAsV2 = true;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce Orders API v1");
    });

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");
app.MapearRotasPedidos();

app.Run();