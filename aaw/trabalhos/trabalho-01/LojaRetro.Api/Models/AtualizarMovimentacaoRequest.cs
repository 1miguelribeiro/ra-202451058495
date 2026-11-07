using System.ComponentModel.DataAnnotations;

namespace LojaRetro.Api.Models;

/// <summary>
/// Dados para corrigir uma movimentação. O jogo não pode ser trocado.
/// </summary>
public class AtualizarMovimentacaoRequest
{
    /// <summary>
    /// Entrada ou Saida.
    /// </summary>
    /// <example>Entrada</example>
    [EnumDataType(typeof(TipoMovimentacao), ErrorMessage = "O tipo deve ser Entrada ou Saida.")]
    public TipoMovimentacao Tipo { get; set; }

    /// <summary>
    /// Quantidade de unidades. Deve ser maior que zero.
    /// </summary>
    /// <example>3</example>
    public int Quantidade { get; set; }

    /// <summary>
    /// Motivo da movimentação.
    /// </summary>
    /// <example>Correção de lançamento</example>
    [Required(ErrorMessage = "O motivo é obrigatório.")]
    public string Motivo { get; set; } = string.Empty;
}
