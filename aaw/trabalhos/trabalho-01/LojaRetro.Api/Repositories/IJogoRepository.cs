using LojaRetro.Api.Models;

namespace LojaRetro.Api.Repositories;

public interface IJogoRepository
{
    Task<Pagina<Jogo>> ListarAsync(int pagina, int tamanho);
    Task<Jogo?> BuscarPorIdAsync(string id);
    Task CriarAsync(Jogo jogo);
    Task AtualizarAsync(Jogo jogo);
    Task<bool> RemoverAsync(string id);
}
