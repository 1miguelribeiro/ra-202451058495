using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using LojaRetro.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace LojaRetro.Api.Controllers;

/// <summary>
/// Entradas e saídas do estoque.
/// </summary>
[ApiController]
[Route("api/movimentacoes")]
[Produces("application/json")]
[Tags("Movimentações")]
public class MovimentacoesController : ControllerBase
{
    private readonly IMovimentacaoRepository _repositorio;
    private readonly EstoqueService _estoque;

    public MovimentacoesController(IMovimentacaoRepository repositorio, EstoqueService estoque)
    {
        _repositorio = repositorio;
        _estoque = estoque;
    }

    /// <summary>
    /// Lista todas as movimentações, da mais recente para a mais antiga.
    /// </summary>
    /// <returns>Lista de movimentações.</returns>
    /// <response code="200">Lista de movimentações (pode vir vazia).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Movimentacao>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Movimentacao>>> Listar()
    {
        var movimentacoes = await _repositorio.ListarAsync();
        return Ok(movimentacoes);
    }

    /// <summary>
    /// Busca uma movimentação pelo id.
    /// </summary>
    /// <param name="id">Id da movimentação.</param>
    /// <returns>Dados da movimentação encontrada.</returns>
    /// <response code="200">Movimentação encontrada.</response>
    /// <response code="400">Id em formato inválido.</response>
    /// <response code="404">Movimentação não encontrada.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Movimentacao), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Movimentacao>> BuscarPorId(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest(new { mensagem = "Id inválido." });

        var movimentacao = await _repositorio.BuscarPorIdAsync(id);
        if (movimentacao is null)
            return NotFound(new { mensagem = "Movimentação não encontrada." });

        return Ok(movimentacao);
    }

    /// <summary>
    /// Registra uma entrada ou saída e atualiza o estoque do jogo.
    /// </summary>
    /// <remarks>
    /// Entrada soma no estoque; saída subtrai. Não é possível vender mais do que há em estoque.
    /// </remarks>
    /// <param name="request">Dados da movimentação.</param>
    /// <returns>Movimentação registrada.</returns>
    /// <response code="201">Movimentação registrada.</response>
    /// <response code="400">Quantidade inválida, id inválido ou estoque insuficiente.</response>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Movimentacao), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Movimentacao>> Registrar(MovimentacaoRequest request)
    {
        try
        {
            var movimentacao = await _estoque.RegistrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = movimentacao.Id }, movimentacao);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
