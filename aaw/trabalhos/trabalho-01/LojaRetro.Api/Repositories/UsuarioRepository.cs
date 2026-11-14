using LojaRetro.Api.Models;
using MongoDB.Driver;

namespace LojaRetro.Api.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IMongoCollection<Usuario> _colecao;

    public UsuarioRepository(IMongoDatabase database)
    {
        _colecao = database.GetCollection<Usuario>("usuarios");
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        return await _colecao.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task CriarAsync(Usuario usuario)
    {
        await _colecao.InsertOneAsync(usuario);
    }
}
