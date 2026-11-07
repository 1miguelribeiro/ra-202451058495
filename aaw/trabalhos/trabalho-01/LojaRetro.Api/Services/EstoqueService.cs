using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using MongoDB.Bson;

namespace LojaRetro.Api.Services;

public class EstoqueService
{
    private readonly IJogoRepository _jogos;
    private readonly IMovimentacaoRepository _movimentacoes;

    public EstoqueService(IJogoRepository jogos, IMovimentacaoRepository movimentacoes)
    {
        _jogos = jogos;
        _movimentacoes = movimentacoes;
    }

    public async Task<Movimentacao> RegistrarAsync(MovimentacaoRequest request)
    {
        if (request.Quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        if (!ObjectId.TryParse(request.JogoId, out _))
            throw new ArgumentException("Id do jogo inválido.");

        var jogo = await _jogos.BuscarPorIdAsync(request.JogoId)
            ?? throw new KeyNotFoundException("Jogo não encontrado.");

        if (request.Tipo == TipoMovimentacao.Saida && request.Quantidade > jogo.QuantidadeEmEstoque)
            throw new InvalidOperationException("Estoque insuficiente.");

        if (request.Tipo == TipoMovimentacao.Entrada)
            jogo.QuantidadeEmEstoque += request.Quantidade;
        else
            jogo.QuantidadeEmEstoque -= request.Quantidade;

        var movimentacao = new Movimentacao
        {
            JogoId = request.JogoId,
            Tipo = request.Tipo,
            Quantidade = request.Quantidade,
            Motivo = request.Motivo,
            Data = DateTime.UtcNow
        };

        await _jogos.AtualizarAsync(jogo);
        await _movimentacoes.CriarAsync(movimentacao);
        return movimentacao;
    }

    public async Task<Movimentacao> AtualizarAsync(string id, AtualizarMovimentacaoRequest request)
    {
        if (!ObjectId.TryParse(id, out _))
            throw new ArgumentException("Id inválido.");

        if (request.Quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        var movimentacao = await _movimentacoes.BuscarPorIdAsync(id)
            ?? throw new KeyNotFoundException("Movimentação não encontrada.");

        var jogo = await _jogos.BuscarPorIdAsync(movimentacao.JogoId)
            ?? throw new KeyNotFoundException("Jogo não encontrado.");

        var novoEstoque = jogo.QuantidadeEmEstoque
            - Efeito(movimentacao.Tipo, movimentacao.Quantidade)
            + Efeito(request.Tipo, request.Quantidade);

        if (novoEstoque < 0)
            throw new InvalidOperationException("Estoque insuficiente.");

        jogo.QuantidadeEmEstoque = novoEstoque;
        movimentacao.Tipo = request.Tipo;
        movimentacao.Quantidade = request.Quantidade;
        movimentacao.Motivo = request.Motivo;

        await _jogos.AtualizarAsync(jogo);
        await _movimentacoes.AtualizarAsync(movimentacao);
        return movimentacao;
    }

    public async Task RemoverAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            throw new ArgumentException("Id inválido.");

        var movimentacao = await _movimentacoes.BuscarPorIdAsync(id)
            ?? throw new KeyNotFoundException("Movimentação não encontrada.");

        var jogo = await _jogos.BuscarPorIdAsync(movimentacao.JogoId)
            ?? throw new KeyNotFoundException("Jogo não encontrado.");

        var novoEstoque = jogo.QuantidadeEmEstoque - Efeito(movimentacao.Tipo, movimentacao.Quantidade);

        if (novoEstoque < 0)
            throw new InvalidOperationException("Não é possível remover: o estoque ficaria negativo.");

        jogo.QuantidadeEmEstoque = novoEstoque;

        await _jogos.AtualizarAsync(jogo);
        await _movimentacoes.RemoverAsync(id);
    }

    private static int Efeito(TipoMovimentacao tipo, int quantidade)
    {
        return tipo == TipoMovimentacao.Entrada ? quantidade : -quantidade;
    }
}
