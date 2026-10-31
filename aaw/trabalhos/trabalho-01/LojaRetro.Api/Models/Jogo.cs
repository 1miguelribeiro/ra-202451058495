using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LojaRetro.Api.Models;

/// <summary>
/// Jogo à venda na loja. Cada jogo é um documento na coleção "jogos" do MongoDB.
/// </summary>
public class Jogo
{
    /// <summary>
    /// Identificador único do jogo, gerado pelo MongoDB.
    /// </summary>
    /// <example>6acd63cf93474b0375eb8ed9</example>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Nome do jogo.
    /// </summary>
    /// <example>Super Mario World</example>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Console do jogo (ex.: Super Nintendo, Mega Drive).
    /// </summary>
    /// <example>Super Nintendo</example>
    public string Console { get; set; } = string.Empty;

    /// <summary>
    /// Ano de lançamento do jogo.
    /// </summary>
    /// <example>1990</example>
    public int AnoLancamento { get; set; }

    /// <summary>
    /// Preço de venda, em reais.
    /// </summary>
    /// <example>199.90</example>
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Preco { get; set; }

    /// <summary>
    /// Quantidade disponível. Só muda por entradas e saídas.
    /// </summary>
    /// <example>4</example>
    public int QuantidadeEmEstoque { get; set; }
}
