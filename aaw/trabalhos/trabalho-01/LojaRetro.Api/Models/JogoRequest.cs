using System.ComponentModel.DataAnnotations;

namespace LojaRetro.Api.Models;

public class JogoRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O console é obrigatório.")]
    public string Console { get; set; } = string.Empty;

    [Range(1970, 2100, ErrorMessage = "O ano de lançamento deve estar entre 1970 e 2100.")]
    public int AnoLancamento { get; set; }

    [Range(0.01, 100000, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }
}
