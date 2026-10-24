using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models;

/// <summary>
/// Representa o empréstimo de um livro a um usuário.
/// </summary>
public class Emprestimo
{
    /// <summary>
    /// Identificador único do empréstimo.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// ID do livro emprestado.
    /// </summary>
    /// <example>1</example>
    [Required]
    public int LivroId { get; set; }

    /// <summary>
    /// Nome do usuário que pegou o livro.
    /// </summary>
    /// <example>João Silva</example>
    [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
    [StringLength(200)]
    public string NomeUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora (UTC) em que o empréstimo foi realizado.
    /// </summary>
    /// <example>2026-10-22T19:00:00Z</example>
    public DateTime DataEmprestimo { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data prevista para a devolução (padrão: 14 dias após o empréstimo).
    /// </summary>
    /// <example>2026-11-05T19:00:00Z</example>
    public DateTime DataDevolucaoPrevista { get; set; } = DateTime.UtcNow.AddDays(14);

    /// <summary>
    /// Data em que o livro foi efetivamente devolvido (nula enquanto não devolvido).
    /// </summary>
    /// <example>null</example>
    public DateTime? DataDevolucaoEfetiva { get; set; }

    /// <summary>
    /// Situação atual do empréstimo: 0 = Ativo, 1 = Devolvido, 2 = Atrasado.
    /// </summary>
    /// <example>0</example>
    public StatusEmprestimo Status { get; set; } = StatusEmprestimo.Ativo;
}

/// <summary>
/// Situações possíveis de um empréstimo.
/// </summary>
public enum StatusEmprestimo
{
    /// <summary>Livro está com o usuário, dentro do prazo.</summary>
    Ativo,
    /// <summary>Livro já foi devolvido.</summary>
    Devolvido,
    /// <summary>Prazo de devolução vencido.</summary>
    Atrasado
}
