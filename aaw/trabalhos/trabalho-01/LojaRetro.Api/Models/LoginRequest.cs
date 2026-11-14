using System.ComponentModel.DataAnnotations;

namespace LojaRetro.Api.Models;

/// <summary>
/// Dados para entrar no sistema.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    /// <example>ana@lojaretro.com</example>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    /// <example>senha123</example>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}
