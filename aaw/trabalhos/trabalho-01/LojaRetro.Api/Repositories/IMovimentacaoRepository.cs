using LojaRetro.Api.Models;

namespace LojaRetro.Api.Repositories;

public interface IMovimentacaoRepository
{
    Task<List<Movimentacao>> ListarAsync();
    Task<Movimentacao?> BuscarPorIdAsync(string id);
    Task CriarAsync(Movimentacao movimentacao);
}
