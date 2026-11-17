using LojaRetro.Api.Models;
using MongoDB.Driver;

namespace LojaRetro.Api.Repositories;

public class JogoRepository : IJogoRepository
{
    private readonly IMongoCollection<Jogo> _colecao;

    public JogoRepository(IMongoDatabase database)
    {
        _colecao = database.GetCollection<Jogo>("jogos");
    }

    public async Task<Pagina<Jogo>> ListarAsync(int pagina, int tamanho)
    {
        var total = await _colecao.CountDocumentsAsync(_ => true);
        var itens = await _colecao.Find(_ => true)
            .SortBy(j => j.Nome)
            .Skip((pagina - 1) * tamanho)
            .Limit(tamanho)
            .ToListAsync();

        return new Pagina<Jogo> { NumeroPagina = pagina, Tamanho = tamanho, Total = total, Itens = itens };
    }

    public async Task<Jogo?> BuscarPorIdAsync(string id)
    {
        return await _colecao.Find(j => j.Id == id).FirstOrDefaultAsync();
    }

    public async Task CriarAsync(Jogo jogo)
    {
        await _colecao.InsertOneAsync(jogo);
    }

    public async Task AtualizarAsync(Jogo jogo)
    {
        await _colecao.ReplaceOneAsync(j => j.Id == jogo.Id, jogo);
    }

    public async Task<bool> RemoverAsync(string id)
    {
        var resultado = await _colecao.DeleteOneAsync(j => j.Id == id);
        return resultado.DeletedCount > 0;
    }
}
