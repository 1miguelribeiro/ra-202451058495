namespace BibliotecaApi.Models;

/// <summary>
/// Envelope de resposta para listagens paginadas.
/// </summary>
/// <typeparam name="T">Tipo dos itens da página.</typeparam>
public class RespostaPaginada<T>
{
    /// <summary>
    /// Itens da página atual.
    /// </summary>
    public List<T> Itens { get; set; } = new();

    /// <summary>
    /// Número da página atual (começa em 1).
    /// </summary>
    /// <example>1</example>
    public int Pagina { get; set; }

    /// <summary>
    /// Quantidade de itens por página (máximo 50).
    /// </summary>
    /// <example>10</example>
    public int TamanhoPagina { get; set; }

    /// <summary>
    /// Total de itens existentes.
    /// </summary>
    /// <example>5</example>
    public int TotalItens { get; set; }

    /// <summary>
    /// Total de páginas disponíveis.
    /// </summary>
    /// <example>1</example>
    public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / TamanhoPagina);
}
