using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using EcommerceOrders.Application.DTOs.Requests;
using EcommerceOrders.Application.Interfaces.Repositories;
using EcommerceOrders.Application.Services;
using EcommerceOrders.Domain.Constants;
using EcommerceOrders.Domain.Entities;
using EcommerceOrders.Domain.Enums;
using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Tests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly PedidoService _pedidoService;

    public PedidoServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _pedidoService = new PedidoService(_pedidoRepositoryMock.Object);
    }

    #region 1. Cenários de Criação de Pedido

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveRetornarPedidoIniciadoComValorTotalCalculado()
    {
        // Arrange
        var request = new CriarPedidoRequest
        {
            CompradorId = Guid.NewGuid(),
            Itens = new List<ItemPedidoRequest>
            {
                new() { ProdutoId = Guid.NewGuid(), NomeProduto = "Mouse Gamer", PrecoUnitario = 150m, Quantidade = 2 },
                new() { ProdutoId = Guid.NewGuid(), NomeProduto = "Teclado Mecânico", PrecoUnitario = 300m, Quantidade = 1 }
            }
        };

        // Act 
        var resultado = await _pedidoService.CriarAsync(request);

        // Assert 
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusPedido.Iniciado);
        resultado.StatusDescricao.Should().Be("Iniciado");
        resultado.ValorTotalPedido.Should().Be(600m);
        resultado.Itens.Should().HaveCount(2);

        _pedidoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()), Times.Once);
        _pedidoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_SemComprador_DeveLancarRegraDeNegocioException()
    {
        // Arrange
        var request = new CriarPedidoRequest
        {
            CompradorId = Guid.Empty,
            Itens = new List<ItemPedidoRequest>
            {
                new() { ProdutoId = Guid.NewGuid(), NomeProduto = "Monitor", PrecoUnitario = 1000m, Quantidade = 1 }
            }
        };

        // Act & Assert
        Func<Task> acao = async () => await _pedidoService.CriarAsync(request);

        await acao.Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage(MensagensDeNegocio.CompradorObrigatorio);
    }

    [Fact]
    public async Task CriarAsync_SemItens_DeveLancarRegraDeNegocioException()
    {
        // Arrange
        var request = new CriarPedidoRequest
        {
            CompradorId = Guid.NewGuid(),
            Itens = new List<ItemPedidoRequest>()
        };

        // Act & Assert
        Func<Task> acao = async () => await _pedidoService.CriarAsync(request);

        await acao.Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage(MensagensDeNegocio.PedidoSemProdutos);
    }

    [Fact]
    public async Task CriarAsync_ComPrecoInvalido_DeveLancarRegraDeNegocioException()
    {
        // Arrange
        var request = new CriarPedidoRequest
        {
            CompradorId = Guid.NewGuid(),
            Itens = new List<ItemPedidoRequest>
            {
                new() { ProdutoId = Guid.NewGuid(), NomeProduto = "Cabo USB", PrecoUnitario = 0m, Quantidade = 1 }
            }
        };

        // Act & Assert
        Func<Task> acao = async () => await _pedidoService.CriarAsync(request);

        await acao.Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage(MensagensDeNegocio.PrecoProdutoInvalido("Cabo USB"));
    }

    #endregion

    #region 2. Cenários de Alteração de Pedido

    [Fact]
    public async Task AlterarAsync_ComPedidoIniciado_DeveAlterarItensComSucesso()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = new Pedido
        {
            Id = pedidoId,
            CompradorId = Guid.NewGuid(),
            Status = StatusPedido.Iniciado,
            Itens = new List<ItemPedido>
            {
                new() { ProdutoId = Guid.NewGuid(), NomeProduto = "Item Antigo", PrecoUnitario = 50m, Quantidade = 1 }
            }
        };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedidoExistente);

        var request = new AlterarPedidoRequest
        {
            Itens = new List<ItemPedidoRequest>
            {
                new() { ProdutoId = Guid.NewGuid(), NomeProduto = "Item Novo", PrecoUnitario = 200m, Quantidade = 2 }
            }
        };

        // Act
        var resultado = await _pedidoService.AlterarAsync(pedidoId, request);

        // Assert
        resultado.Itens.Should().HaveCount(1);
        resultado.Itens[0].NomeProduto.Should().Be("Item Novo");
        resultado.ValorTotalPedido.Should().Be(400m);
        resultado.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public async Task AlterarAsync_ComPedidoJaProcessado_DeveLancarRegraDeNegocioException()
    {
        // Arrange:
        var pedidoId = Guid.NewGuid();
        var pedidoProcessado = new Pedido
        {
            Id = pedidoId,
            CompradorId = Guid.NewGuid(),
            Status = StatusPedido.Processado
        };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedidoProcessado);

        var request = new AlterarPedidoRequest
        {
            Itens = new List<ItemPedidoRequest>
            {
                new() { ProdutoId = Guid.NewGuid(), NomeProduto = "Item", PrecoUnitario = 100m, Quantidade = 1 }
            }
        };

        // Act & Assert
        Func<Task> acao = async () => await _pedidoService.AlterarAsync(pedidoId, request);

        await acao.Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage(MensagensDeNegocio.ApenasPedidosNaoProcessadosPodemSerAlterados);
    }

    #endregion

    #region 3. Cenários de Cancelamento de Pedido

    [Fact]
    public async Task CancelarAsync_ComStatusIniciado_DeveCancelarComSucesso()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Iniciado };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        // Act
        var resultado = await _pedidoService.CancelarAsync(pedidoId);

        // Assert
        resultado.Status.Should().Be(StatusPedido.Cancelado);
        resultado.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelarAsync_ComStatusProcessado_DeveCancelarComSucesso()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Processado };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        // Act
        var resultado = await _pedidoService.CancelarAsync(pedidoId);

        // Assert
        resultado.Status.Should().Be(StatusPedido.Cancelado);
    }

    [Fact]
    public async Task CancelarAsync_ComStatusEnviado_DeveLancarRegraDeNegocioException()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoEnviado = new Pedido { Id = pedidoId, Status = StatusPedido.Enviado };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedidoEnviado);

        // Act & Assert
        Func<Task> acao = async () => await _pedidoService.CancelarAsync(pedidoId);

        await acao.Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage(MensagensDeNegocio.ApenasPedidosIniciadosOuProcessadosPodemSerCancelados);
    }

    #endregion

    #region 4. Cenários de Envio de Pedido

    [Fact]
    public async Task EnviarAsync_ComStatusProcessado_DeveEnviarComSucesso()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Processado };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        // Act
        var resultado = await _pedidoService.EnviarAsync(pedidoId);

        // Assert
        resultado.Status.Should().Be(StatusPedido.Enviado);
    }

    [Fact]
    public async Task EnviarAsync_ComStatusIniciado_DeveLancarRegraDeNegocioException()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoIniciado = new Pedido { Id = pedidoId, Status = StatusPedido.Iniciado };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedidoIniciado);

        // Act & Assert
        Func<Task> acao = async () => await _pedidoService.EnviarAsync(pedidoId);

        await acao.Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage(MensagensDeNegocio.ApenasPedidosProcessadosPodemSerEnviados);
    }

    #endregion

    #region 5. Cenários de Processamento de Pedido

    [Fact]
    public async Task ProcessarAsync_ComStatusIniciado_DeveProcessarComSucesso()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedido = new Pedido { Id = pedidoId, Status = StatusPedido.Iniciado };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        // Act
        var resultado = await _pedidoService.ProcessarAsync(pedidoId);

        // Assert
        resultado.Status.Should().Be(StatusPedido.Processado);
        resultado.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public async Task ProcessarAsync_ComStatusInvalido_DeveLancarRegraDeNegocioException()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoCancelado = new Pedido { Id = pedidoId, Status = StatusPedido.Cancelado };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedidoCancelado);

        // Act & Assert
        Func<Task> acao = async () => await _pedidoService.ProcessarAsync(pedidoId);

        await acao.Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage(MensagensDeNegocio.StatusInvalidoParaProcessamento(StatusPedido.Cancelado.ToString()));
    }

    #endregion
}