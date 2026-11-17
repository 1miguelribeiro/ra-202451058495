namespace LojaRetro.Api.Models;

/// <summary>
/// Uma página de resultados de uma lista.
/// </summary>
public class Pagina<T>
{
    /// <summary>
    /// Número da página atual.
    /// </summary>
    /// <example>1</example>
    public int NumeroPagina { get; set; }

    /// <summary>
    /// Quantidade máxima de itens por página.
    /// </summary>
    /// <example>10</example>
    public int Tamanho { get; set; }

    /// <summary>
    /// Total de itens em todas as páginas.
    /// </summary>
    /// <example>23</example>
    public long Total { get; set; }

    /// <summary>
    /// Quantidade de páginas.
    /// </summary>
    /// <example>3</example>
    public int TotalDePaginas => (int)Math.Ceiling(Total / (double)Tamanho);

    /// <summary>
    /// Itens desta página.
    /// </summary>
    public List<T> Itens { get; set; } = new();
}
