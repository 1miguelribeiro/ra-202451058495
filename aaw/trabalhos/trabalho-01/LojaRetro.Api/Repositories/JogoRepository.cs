using LojaRetro.Api.Models;
using MongoDB.Driver;

namespace LojaRetro.Api.Repositories;

public class JogoRepository
{
    private readonly IMongoCollection<Jogo> _colecao;

    public JogoRepository(IMongoDatabase database)
    {
        _colecao = database.GetCollection<Jogo>("jogos");
    }

    public async Task<List<Jogo>> ListarAsync()
    {
        return await _colecao.Find(_ => true).ToListAsync();
    }

    public async Task<Jogo?> BuscarPorIdAsync(string id)
    {
        return await _colecao.Find(j => j.Id == id).FirstOrDefaultAsync();
    }

    public async Task CriarAsync(Jogo jogo)
    {
        await _colecao.InsertOneAsync(jogo);
    }
}
