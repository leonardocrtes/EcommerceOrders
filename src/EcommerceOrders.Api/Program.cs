using EcommerceOrders.Api.Endpoints;
using EcommerceOrders.Api.Middlewares;
using EcommerceOrders.Application.Interfaces.Services;
using EcommerceOrders.Application.Services;
using EcommerceOrders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Health Check
builder.Services.AddHealthChecks();

// 3. Tratamento Global de Erros 
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratamentoGlobalErrosHandler>();

// 4. Injeção de Dependência das Camadas
builder.Services.AdicionarInfraestrutura(builder.Configuration);
builder.Services.AddScoped<IPedidoService, PedidoService>();

var app = builder.Build();

// 5. Configuração dos Middlewares
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 6. Mapeamento das Rotas
app.MapHealthChecks("/health");
app.MapearRotasPedidos();

app.Run();