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

    public async Task<List<Movimentacao>> ListarAsync()
    {
        return await _colecao.Find(_ => true).SortByDescending(m => m.Data).ToListAsync();
    }

    public async Task<Movimentacao?> BuscarPorIdAsync(string id)
    {
        return await _colecao.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    public async Task CriarAsync(Movimentacao movimentacao)
    {
        await _colecao.InsertOneAsync(movimentacao);
    }
}
