using System.ComponentModel.DataAnnotations;

namespace LojaRetro.Api.Models;

/// <summary>
/// Dados para registrar uma entrada ou saída.
/// </summary>
public class MovimentacaoRequest
{
    /// <summary>
    /// Id do jogo movimentado.
    /// </summary>
    /// <example>6acd63cf93474b0375eb8ed9</example>
    [Required(ErrorMessage = "O id do jogo é obrigatório.")]
    public string JogoId { get; set; } = string.Empty;

    /// <summary>
    /// Entrada ou Saida.
    /// </summary>
    /// <example>Entrada</example>
    [EnumDataType(typeof(TipoMovimentacao), ErrorMessage = "O tipo deve ser Entrada ou Saida.")]
    public TipoMovimentacao Tipo { get; set; }

    /// <summary>
    /// Quantidade de unidades. Deve ser maior que zero.
    /// </summary>
    /// <example>5</example>
    public int Quantidade { get; set; }

    /// <summary>
    /// Motivo da movimentação.
    /// </summary>
    /// <example>Compra de fornecedor</example>
    [Required(ErrorMessage = "O motivo é obrigatório.")]
    public string Motivo { get; set; } = string.Empty;
}
