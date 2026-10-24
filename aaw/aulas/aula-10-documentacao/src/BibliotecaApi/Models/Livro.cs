using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models;

/// <summary>
/// Representa um livro do acervo da biblioteca.
/// </summary>
public class Livro
{
    /// <summary>
    /// Identificador único do livro.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Título do livro.
    /// </summary>
    /// <example>Dom Casmurro</example>
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(300, MinimumLength = 1)]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Código ISBN do livro.
    /// </summary>
    /// <example>978-8535910663</example>
    [StringLength(20)]
    public string? Isbn { get; set; }

    /// <summary>
    /// Ano de publicação (entre 1000 e 2100).
    /// </summary>
    /// <example>1899</example>
    [Range(1000, 2100, ErrorMessage = "Ano de publicação inválido.")]
    public int AnoPublicacao { get; set; }

    /// <summary>
    /// Gênero literário do livro.
    /// </summary>
    /// <example>Romance</example>
    [StringLength(100)]
    public string? Genero { get; set; }

    /// <summary>
    /// ID do autor do livro.
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "O autor é obrigatório.")]
    public int AutorId { get; set; }

    /// <summary>
    /// Indica se o livro está disponível para empréstimo.
    /// </summary>
    /// <example>true</example>
    public bool Disponivel { get; set; } = true;
}
