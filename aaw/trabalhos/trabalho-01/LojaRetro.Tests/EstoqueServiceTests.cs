using FluentAssertions;
using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using LojaRetro.Api.Services;
using Moq;
using Xunit;

namespace LojaRetro.Tests;

public class EstoqueServiceTests
{
    private const string JogoId = "6acd63cf93474b0375eb8ed9";
    private const string MovimentacaoId = "6acd63cf93474b0375eb8eda";

    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IMovimentacaoRepository> _movimentacoes = new();
    private readonly EstoqueService _service;

    public EstoqueServiceTests()
    {
        _service = new EstoqueService(_jogos.Object, _movimentacoes.Object);
    }

    private Jogo JogoComEstoque(int quantidade)
    {
        var jogo = new Jogo { Id = JogoId, Nome = "Super Mario World", QuantidadeEmEstoque = quantidade };
        _jogos.Setup(r => r.BuscarPorIdAsync(JogoId)).ReturnsAsync(jogo);
        return jogo;
    }

    private static MovimentacaoRequest Pedido(TipoMovimentacao tipo, int quantidade)
    {
        return new MovimentacaoRequest { JogoId = JogoId, Tipo = tipo, Quantidade = quantidade, Motivo = "Teste" };
    }

    [Fact(DisplayName = "Entrada de 5 unidades aumenta o estoque")]
    public async Task Registrar_Entrada_AumentaEstoque()
    {
        var jogo = JogoComEstoque(2);

        await _service.RegistrarAsync(Pedido(TipoMovimentacao.Entrada, 5));

        jogo.QuantidadeEmEstoque.Should().Be(7);
        _jogos.Verify(r => r.AtualizarAsync(jogo), Times.Once);
        _movimentacoes.Verify(r => r.CriarAsync(It.IsAny<Movimentacao>()), Times.Once);
    }

    [Fact(DisplayName = "Saída de 2 unidades diminui o estoque")]
    public async Task Registrar_Saida_DiminuiEstoque()
    {
        var jogo = JogoComEstoque(5);

        await _service.RegistrarAsync(Pedido(TipoMovimentacao.Saida, 2));

        jogo.QuantidadeEmEstoque.Should().Be(3);
    }

    [Fact(DisplayName = "Saída igual ao estoque deixa o estoque em zero")]
    public async Task Registrar_SaidaDeTudo_ZeraEstoque()
    {
        var jogo = JogoComEstoque(3);

        await _service.RegistrarAsync(Pedido(TipoMovimentacao.Saida, 3));

        jogo.QuantidadeEmEstoque.Should().Be(0);
    }

    [Fact(DisplayName = "Saída maior que o estoque é recusada e nada é gravado")]
    public async Task Registrar_SaidaMaiorQueEstoque_LancaErro()
    {
        var jogo = JogoComEstoque(1);

        var acao = () => _service.RegistrarAsync(Pedido(TipoMovimentacao.Saida, 3));

        await acao.Should().ThrowAsync<InvalidOperationException>().WithMessage("Estoque insuficiente.");
        jogo.QuantidadeEmEstoque.Should().Be(1);
        _jogos.Verify(r => r.AtualizarAsync(It.IsAny<Jogo>()), Times.Never);
        _movimentacoes.Verify(r => r.CriarAsync(It.IsAny<Movimentacao>()), Times.Never);
    }

    [Theory(DisplayName = "Quantidade zero ou negativa é recusada")]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Registrar_QuantidadeInvalida_LancaErro(int quantidade)
    {
        var acao = () => _service.RegistrarAsync(Pedido(TipoMovimentacao.Entrada, quantidade));

        await acao.Should().ThrowAsync<ArgumentException>().WithMessage("A quantidade deve ser maior que zero.");
        _jogos.Verify(r => r.BuscarPorIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Movimentação de jogo que não existe é recusada")]
    public async Task Registrar_JogoInexistente_LancaErro()
    {
        _jogos.Setup(r => r.BuscarPorIdAsync(JogoId)).ReturnsAsync((Jogo?)null);

        var acao = () => _service.RegistrarAsync(Pedido(TipoMovimentacao.Entrada, 1));

        await acao.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Jogo não encontrado.");
    }

    [Fact(DisplayName = "Remover uma entrada já vendida é recusado, pois o estoque ficaria negativo")]
    public async Task Remover_EntradaJaVendida_LancaErro()
    {
        JogoComEstoque(1);
        _movimentacoes.Setup(r => r.BuscarPorIdAsync(MovimentacaoId)).ReturnsAsync(new Movimentacao
        {
            Id = MovimentacaoId,
            JogoId = JogoId,
            Tipo = TipoMovimentacao.Entrada,
            Quantidade = 5
        });

        var acao = () => _service.RemoverAsync(MovimentacaoId);

        await acao.Should().ThrowAsync<InvalidOperationException>();
        _movimentacoes.Verify(r => r.RemoverAsync(It.IsAny<string>()), Times.Never);
    }
}
