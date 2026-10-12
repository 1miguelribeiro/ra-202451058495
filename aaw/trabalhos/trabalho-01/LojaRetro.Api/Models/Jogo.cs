using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LojaRetro.Api.Models;

public class Jogo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Console { get; set; } = string.Empty;

    public int AnoLancamento { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Preco { get; set; }

    public int QuantidadeEmEstoque { get; set; }
}
