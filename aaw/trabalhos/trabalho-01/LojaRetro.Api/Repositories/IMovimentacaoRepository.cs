using LojaRetro.Api.Models;

namespace LojaRetro.Api.Repositories;

public interface IMovimentacaoRepository
{
    Task<Pagina<Movimentacao>> ListarAsync(string? jogoId, int pagina, int tamanho);
    Task<List<Movimentacao>> ListarPorJogoAsync(string jogoId);
    Task<Movimentacao?> BuscarPorIdAsync(string id);
    Task CriarAsync(Movimentacao movimentacao);
    Task AtualizarAsync(Movimentacao movimentacao);
    Task<bool> RemoverAsync(string id);
}
