using LojaRetro.Api.Models;

namespace LojaRetro.Api.Repositories;

public interface IMovimentacaoRepository
{
    Task<List<Movimentacao>> ListarAsync();
    Task<List<Movimentacao>> ListarPorJogoAsync(string jogoId);
    Task<Movimentacao?> BuscarPorIdAsync(string id);
    Task CriarAsync(Movimentacao movimentacao);
    Task AtualizarAsync(Movimentacao movimentacao);
    Task<bool> RemoverAsync(string id);
}
