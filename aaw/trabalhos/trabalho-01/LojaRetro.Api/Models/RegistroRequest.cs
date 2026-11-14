using System.ComponentModel.DataAnnotations;

namespace LojaRetro.Api.Models;

/// <summary>
/// Dados para criar um usuário. Todo usuário novo nasce com o perfil funcionario.
/// </summary>
public class RegistroRequest
{
    /// <summary>
    /// Nome do usuário.
    /// </summary>
    /// <example>Ana</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// E-mail usado no login.
    /// </summary>
    /// <example>ana@lojaretro.com</example>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha com pelo menos 6 caracteres.
    /// </summary>
    /// <example>senha123</example>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}
