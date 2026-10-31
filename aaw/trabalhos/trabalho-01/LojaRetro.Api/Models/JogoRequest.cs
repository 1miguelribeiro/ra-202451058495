using System.ComponentModel.DataAnnotations;

namespace LojaRetro.Api.Models;

/// <summary>
/// Dados para cadastrar ou alterar um jogo.
/// </summary>
public class JogoRequest
{
    /// <summary>
    /// Nome do jogo.
    /// </summary>
    /// <example>Super Mario World</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Console do jogo (ex.: Super Nintendo, Mega Drive).
    /// </summary>
    /// <example>Super Nintendo</example>
    [Required(ErrorMessage = "O console é obrigatório.")]
    public string Console { get; set; } = string.Empty;

    /// <summary>
    /// Ano de lançamento do jogo.
    /// </summary>
    /// <example>1990</example>
    [Range(1970, 2100, ErrorMessage = "O ano de lançamento deve estar entre 1970 e 2100.")]
    public int AnoLancamento { get; set; }

    /// <summary>
    /// Preço de venda, em reais.
    /// </summary>
    /// <example>199.90</example>
    [Range(0.01, 100000, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }
}
