# 📦 E-Commerce Orders API

API RESTful desenvolvida em **.NET 8** para gerenciamento de pedidos de e-commerce, aplicando princípios de **Clean Architecture**, **Minimal APIs**, **Entity Framework Core**, **SQL Server** e **Docker**.

---

## 🚀 Tecnologias e Práticas Utilizadas

* **Framework:** .NET 8 (LTS)
* **Padrão de API:** Minimal APIs com agrupamento por rotas (`MapGroup`)
* **Banco de Dados:** SQL Server 2022 via Docker
* **ORM:** Entity Framework Core 8 (com mapeamento Fluent API e Migrations)
* **Arquitetura:** Clean Architecture (Domain, Application, Infrastructure, Api, Tests)
* **Princípios:** SOLID, DRY, KISS, YAGNI e Fail-Fast
* **Documentação Interativa:** Swagger UI e **Scalar API Reference**
* **Tratamento de Erros:** Middleware global no padrão internacional **RFC 7807 (Problem Details)**
* **Observabilidade & Saúde:** Endpoint de **Health Checks** (`/health`)
* **Testes de Unidade:** xUnit, Moq e FluentAssertions

---

## 📐 Arquitetura da Solução

O projeto está modularizado seguindo a separação de responsabilidades em camadas limpas:

```text
EcommerceOrders/
├── src/
│   ├── EcommerceOrders.Domain/          # Entidades (POCOs), Enums, Constantes e Exceções de Domínio
│   ├── EcommerceOrders.Application/     # Casos de Uso, DTOs (Requests/Responses), Interfaces e Services
│   ├── EcommerceOrders.Infrastructure/  # EF Core, AppDbContext, Mapeamentos Fluent API e Repositórios
│   └── EcommerceOrders.Api/             # Minimal APIs, Middlewares, Configurações do Swagger/Scalar
├── tests/
│   └── EcommerceOrders.Tests/           # Testes de unidade com xUnit e Moq
├── docker-compose.yml                   # Orquestração do SQL Server em container
└── README.md                            # Documentação do projeto
```
---

## 💼 Regras de Negócio

A gestão do ciclo de vida dos pedidos respeita as regras e restrições de transição de status:

### Status Suportados:
- 1 - Iniciado: Pedido recebido e criado pelo comprador.
- 2 - Processado: Pedido processado pelo sistema.
- 3 - Enviado: Pedido despachado para entrega.
- 4 - Cancelado: Pedido cancelado.

### Regras de Transição e Integridade:
- Criação: Todo pedido deve conter um comprador válido, pelo menos um produto e preços maiores que zero.
- Alteração: Apenas pedidos não processados (Iniciado) podem ter seus itens alterados.
- Cancelamento: Apenas pedidos com status Iniciado ou Processado podem ser cancelados (pedidos já enviados não podem ser cancelados).
- Envio: Apenas pedidos previamente Processados podem ser marcados como enviados.

---

## 🛣️ Endpoints da API (v1)

Todas as rotas estão versionadas sob o prefixo /api/v1/pedidos:

- POST /api/v1/pedidos
  - Descrição: Cria um novo pedido com seus itens
  - Retorno de Sucesso: 201 Created (com header Location)

- GET /api/v1/pedidos
  - Descrição: Lista pedidos com suporte a filtros (Status, CompradorId, DataInicio, DataFim)
  - Retorno de Sucesso: 200 OK

- GET /api/v1/pedidos/{id}
  - Descrição: Busca um pedido específico por ID
  - Retorno de Sucesso: 200 OK (ou 404 Not Found)

- PUT /api/v1/pedidos/{id}
  - Descrição: Altera os itens de um pedido não processado
  - Retorno de Sucesso: 200 OK

- PATCH /api/v1/pedidos/{id}/processar
  - Descrição: Transiciona o pedido para o status Processado
  - Retorno de Sucesso: 200 OK

- PATCH /api/v1/pedidos/{id}/enviar
  - Descrição: Marca o pedido como Enviado (apenas pedidos processados)
  - Retorno de Sucesso: 200 OK

- PATCH /api/v1/pedidos/{id}/cancelar
  - Descrição: Cancela um pedido iniciado ou processado
  - Retorno de Sucesso: 200 OK

- DELETE /api/v1/pedidos/{id}
  - Descrição: Exclui fisicamente o pedido e seus itens do banco de dados
  - Retorno de Sucesso: 204 No Content

- GET /health
  - Descrição: Checagem de saúde da aplicação e conectividade
  - Retorno de Sucesso: 200 OK (Healthy)

---

## 🐳 Como Executar com Docker

### Pré-requisitos:
- Docker Desktop instalado e em execução.

### 1. Subir o Banco de Dados (SQL Server 2022):
Na raiz do projeto, execute:

```text
docker compose up sqlserver -d
```

### 2. Aplicar as Migrations e Iniciar a API:
Com o container ativo, aplique as migrations no banco de dados:

```text
dotnet ef database update -p src/EcommerceOrders.Infrastructure -s src/EcommerceOrders.Api
```

Em seguida, execute a API:

```text
dotnet run --project src/EcommerceOrders.Api
```
---

## 📖 Documentação Interativa

Com a API em execução, acesse a documentação interativa no navegador:

- Scalar API Reference: https://localhost:7152/scalar/v1
- Swagger UI: https://localhost:7152/swagger
- Health Check: https://localhost:7152/health

---

## 🧪 Execução dos Testes de Unidade

O projeto conta com uma bateria de testes unitários cobrindo todos os fluxos de sucesso e as violações de regras de negócio do PedidoService, utilizando xUnit, Moq e FluentAssertions.

Para executar os testes via terminal:

```text
dotnet test
```
---

## 👤 Autor
Desenvolvido por Leonardo Côrtes
