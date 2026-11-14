using LojaRetro.Api.Models;

namespace LojaRetro.Api.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailAsync(string email);
    Task CriarAsync(Usuario usuario);
}
