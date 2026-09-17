namespace EcommerceOrders.Domain.Constants;

public static class MensagensDeNegocio
{
    public const string CompradorObrigatorio = "O comprador é obrigatório para criar um pedido.";
    public const string PedidoSemProdutos = "Todo pedido deve possuir pelo menos um produto.";
    public const string ApenasPedidosNaoProcessadosPodemSerAlterados = "Apenas pedidos não processados (status Iniciado) podem ser alterados.";
    public const string ApenasPedidosIniciadosOuProcessadosPodemSerCancelados = "Apenas pedidos iniciados ou processados podem ser cancelados.";
    public const string ApenasPedidosProcessadosPodemSerEnviados = "Apenas pedidos processados podem ser enviados.";

    public static string PedidoNaoEncontrado(Guid id) =>
        $"Pedido com ID '{id}' não foi encontrado.";

    public static string PrecoProdutoInvalido(string nome) =>
        $"O preço do produto '{nome}' deve ser maior que zero.";

    public static string QuantidadeProdutoInvalida(string nome) =>
        $"A quantidade do produto '{nome}' deve ser de pelo menos 1.";

    public static string StatusInvalidoParaProcessamento(string statusAtual) =>
        $"Apenas pedidos no status 'Iniciado' podem ser processados. Status atual: '{statusAtual}'.";
}