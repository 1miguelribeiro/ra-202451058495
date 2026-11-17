using LojaRetro.Api.Models;
using MongoDB.Driver;

namespace LojaRetro.Api.Repositories;

public class MovimentacaoRepository : IMovimentacaoRepository
{
    private readonly IMongoCollection<Movimentacao> _colecao;

    public MovimentacaoRepository(IMongoDatabase database)
    {
        _colecao = database.GetCollection<Movimentacao>("movimentacoes");
    }

    public async Task<Pagina<Movimentacao>> ListarAsync(string? jogoId, int pagina, int tamanho)
    {
        var filtro = jogoId is null
            ? Builders<Movimentacao>.Filter.Empty
            : Builders<Movimentacao>.Filter.Eq(m => m.JogoId, jogoId);

        var total = await _colecao.CountDocumentsAsync(filtro);
        var itens = await _colecao.Find(filtro)
            .SortByDescending(m => m.Data)
            .Skip((pagina - 1) * tamanho)
            .Limit(tamanho)
            .ToListAsync();

        return new Pagina<Movimentacao> { NumeroPagina = pagina, Tamanho = tamanho, Total = total, Itens = itens };
    }

    public async Task<List<Movimentacao>> ListarPorJogoAsync(string jogoId)
    {
        return await _colecao.Find(m => m.JogoId == jogoId).SortByDescending(m => m.Data).ToListAsync();
    }

    public async Task<Movimentacao?> BuscarPorIdAsync(string id)
    {
        return await _colecao.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    public async Task CriarAsync(Movimentacao movimentacao)
    {
        await _colecao.InsertOneAsync(movimentacao);
    }

    public async Task AtualizarAsync(Movimentacao movimentacao)
    {
        await _colecao.ReplaceOneAsync(m => m.Id == movimentacao.Id, movimentacao);
    }

    public async Task<bool> RemoverAsync(string id)
    {
        var resultado = await _colecao.DeleteOneAsync(m => m.Id == id);
        return resultado.DeletedCount > 0;
    }
}
