using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LojaRetro.Api.Models;

/// <summary>
/// Entrada ou saída de um jogo no estoque. Cada movimentação é um documento na coleção "movimentacoes" e guarda o id do jogo.
/// </summary>
public class Movimentacao
{
    /// <summary>
    /// Identificador único da movimentação, gerado pelo MongoDB.
    /// </summary>
    /// <example>6b0f1c2a93474b0375eb8f10</example>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Id do jogo movimentado.
    /// </summary>
    /// <example>6acd63cf93474b0375eb8ed9</example>
    [BsonRepresentation(BsonType.ObjectId)]
    public string JogoId { get; set; } = string.Empty;

    /// <summary>
    /// Entrada ou Saida.
    /// </summary>
    /// <example>Entrada</example>
    [BsonRepresentation(BsonType.String)]
    public TipoMovimentacao Tipo { get; set; }

    /// <summary>
    /// Quantidade de unidades movimentadas.
    /// </summary>
    /// <example>5</example>
    public int Quantidade { get; set; }

    /// <summary>
    /// Data e hora da movimentação (UTC).
    /// </summary>
    /// <example>2026-11-01T19:00:00Z</example>
    public DateTime Data { get; set; }

    /// <summary>
    /// Motivo da movimentação.
    /// </summary>
    /// <example>Compra de fornecedor</example>
    public string Motivo { get; set; } = string.Empty;
}
