namespace LojaRetro.Api.Models;

/// <summary>
/// Resposta do login, com o token que deve ir no header Authorization.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Token JWT. Enviar como "Authorization: Bearer {token}".
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Nome do usuário logado.
    /// </summary>
    /// <example>Ana</example>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Perfil do usuário: admin ou funcionario.
    /// </summary>
    /// <example>funcionario</example>
    public string Perfil { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora (UTC) em que o token expira.
    /// </summary>
    public DateTime ExpiraEm { get; set; }
}
